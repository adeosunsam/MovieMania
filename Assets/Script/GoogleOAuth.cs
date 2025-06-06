using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google;
using Newtonsoft.Json;
using UnityEngine;
using static RequestDtos;
using static ResponseDtos;
using static SharedResources;

public class GoogleOAuth : MonoBehaviour
{
    [SerializeField]
    private string userId;

    [SerializeField]
    private GameObject authPanel, pages;

    private bool isExternalCallOn;

    internal bool healthy;

    private void Update()
    {
        if (TopicResponse != null && TopicResponse.Any() && (GamingCount == null || UserFriends == null) && !isExternalCallOn)
        {
            Debug.Log($"RELOADING USER FRIEND AND GAMING COUNT");
            isExternalCallOn = true;
            //Reload();
        }
    }

    private async Task<bool> HasSavedTopic()
    {
        var tcs = new TaskCompletionSource<bool>();

        MainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                bool result = PlayerPrefExtension<List<TopicResponseDto>>.HasKey();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        return await tcs.Task;
    }

    public async void OnSignIn()
    {
        //await GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
        //  OnAuthenticationFinished, TaskScheduler.Default);
        //Disconnect();

        {
            var request = new UserDetailDto
            {
                FirstName = userId,
                LastName = "Adeosun",
                Email = $"{userId}@gmail.com",
                UserName = $"allos_{userId}",
                UserId = $"{userId}",
                Image = "https://res.cloudinary.com/chrismeyer/image/upload/v1652828948/ab71355e-3d40-4ada-95b5-021d08c0e6eeWhatsApp%20Image%202022-03-09%20at%204.37.59%20PM.jpeg.jpg"
             };

            RunJobStart(request);
            authPanel.SetActive(false);
            pages.SetActive(true);

            BroadcastService.Singleton.ConnectUser();
        }
    }

    //IEnumerator LoadProfilePics(UserDetailDto request)
    //{
    //    using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(request.Image))
    //    {
    //        yield return webRequest.SendWebRequest();

    //        if (webRequest.result == UnityWebRequest.Result.ConnectionError
    //            || webRequest.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            request.IsImageLoadingStopped = true;
    //            //request.IsImageLoadingStopped = isImageLoadingStopped;
    //        }
    //        else
    //        {
    //            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

    //            request.Sprite = SpriteFromTexture2D(texture);
    //            //request.Sprite = imageSprite;
    //        }
    //    }
    //}

    public void Disconnect()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        //GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {

        _ = Task.Run(async () =>
        {
            healthy = await ExternalService.HeathCheck();

            if (!healthy && !task.IsCanceled)
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    ToastNotification.Show("Connection error\nCheck your internet connection and try again", "error");
                });
            }
            else if (task.IsFaulted)
            {
                using IEnumerator<Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;

                    Debug.LogError($"Error signing in: {error.Status}, {error.Message}");

                    MainThreadDispatcher.Enqueue(() =>
                    {
                        ToastNotification.Show("Connection error\nCheck your internet connection and try again", "error");
                    });
                }
            }
            else if (task.IsCanceled) { }
            else if (task.IsCompletedSuccessfully)
            {
                var request = new UserDetailDto
                {
                    FirstName = task.Result.GivenName,
                    LastName = task.Result.FamilyName,
                    Email = task.Result.Email,
                    UserName = task.Result.FamilyName,
                    UserId = task.Result.UserId,
                    Image = task.Result.ImageUrl.AbsoluteUri
                };

                Debug.Log($"USER DETAILS: {JsonConvert.SerializeObject(request, Formatting.Indented)}");

                RunJobStart(request);

                MainThreadDispatcher.Enqueue(() =>
                {
                    authPanel.SetActive(false);
                    pages.SetActive(true);

                    BroadcastService.Singleton.ConnectUser();
                });
            }
        });
    }

    public void RunJobStart(UserDetailDto request)
    {
        UserDetail = request;

        StartCoroutine(LoadProfilePics(UserDetail));

        _ = Task.Run(() => ExternalService.Login(request));

        _ = Task.Run(async () =>
        {
            var data = await ExternalService.FetchUserGamingCount(request.UserId);
            GamingCount = data;
        });

        _ = Task.Run(async () =>
        {
            var data = await ExternalService.GetUserFriends(request.UserId);
            UserFriends = data;

            MainThreadDispatcher.Enqueue(() =>
            {
                LoadUserImages();
            });
        });

        //StartCoroutine(SharedResources.LoadProfilePics(UserDetail));

        _ = Task.Run(async () =>
        {
            //var data = await ExternalService.GetUserActivity(SharedResources.UserDetail.UserId);
            var data = await ExternalService.GetUserActivity(request.UserId);
            UserActivity = data;

            Debug.Log($"NO OF USER ACTIVITIES FETCHED {data.Count}");

            UserActivity.ForEach(async x =>
            {
                x.Sprite = await LoadImageAsync(x.UserImage);
            });
        });

        _ = Task.Run(async () =>
        {
            var hasSavedTopic = await HasSavedTopic();

            if (hasSavedTopic)
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    TopicResponse = PlayerPrefExtension<List<TopicResponseDto>>.Get();
                    StartUp();
                });
            }

            var incomingTopics = await ExternalService.FetchAvailableTopics(request.UserId);

            Debug.Log($"NO OF TOPICS FETCHED {incomingTopics.Count}");

            if (hasSavedTopic)
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    PlayerPrefExtension<List<TopicResponseDto>>.UpdateDb(incomingTopics);
                });

                var incomingDict = incomingTopics.ToDictionary(x => x.Id, x => x);
                TopicResponse.ForEach(x =>
                {
                    var incomingData = GetValue(incomingDict, x.Id);
                    if (incomingData == null) return;

                    x.FollowersCount = incomingData.FollowersCount;
                    x.QuestionCount = incomingData.QuestionCount;
                    x.IsFollowed = incomingData.IsFollowed;
                });
            }
            else
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    PlayerPrefExtension<List<TopicResponseDto>>.UpdateDb(incomingTopics);
                });
                TopicResponse = incomingTopics;
                StartUp();
            }
        });


        //remove before building
        //authPanel.SetActive(false);
        //pages.SetActive(true);
    }

    public T GetValue<T>(Dictionary<string, T> data, string key) where T : class
    {
        bool hasValue = data.TryGetValue(key, out T value);

        if (hasValue)
        {
            return value;
        }
        return null;
    }

    public void Reload()
    {
        _ = Task.Run(async () =>
        {
            var data = await ExternalService.FetchUserGamingCount(UserDetail.UserId);
            GamingCount = data;
        });

        _ = Task.Run(async () =>
        {
            var data = await ExternalService.GetUserFriends(UserDetail.UserId);
            UserFriends = data;

            MainThreadDispatcher.Enqueue(() =>
            {
                LoadUserImages();
            });

            isExternalCallOn = false;
        });
    }

    /*public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.LogError("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.LogError("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }*/
}