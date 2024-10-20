using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static RequestDtos;
using static ResponseDtos;
using static SharedResources;

public class GoogleOAuth : MonoBehaviour
{
    //[SerializeField]
    //private TextMeshProUGUI usernameText, fullname, userEmailText, userId, imageUrl, errorText;

    [SerializeField]
    private GameObject authPanel, pages;

    private const string webClientId = "1079234788728-hn0tlk70vq570ap3ea4o4j3mqrn268sj.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private string imageUrl;
    private Sprite imageSprite;

    internal bool isImageLoadingStopped;

    //private bool isRunJob;

    //private UserDetailDto request;
    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private void Update()
    {
        /*if(request != null && !isRunJob)
        {
            isRunJob = true;
            RunJobStart();
        }*/
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
        /*GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        await GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished, TaskScheduler.Default);

        authPanel.SetActive(false);
        pages.SetActive(true);*/

        var request = new UserDetailDto
        {
            FirstName = "Odunayo",// task.Result.DisplayName,
            LastName = "Adeosun",//task.Result.GivenName,
            Email = "adeosunsamsamuel30@gmail.com",//task.Result.Email,
            UserName = "allos",//task.Result.FamilyName,
            UserId = "cb277a70231842a1947596435708e245",//task.Result.UserId,
            //Image = "https://res.cloudinary.com/chrismeyer/image/upload/v1652828948/ab71355e-3d40-4ada-95b5-021d08c0e6eeWhatsApp%20Image%202022-03-09%20at%204.37.59%20PM.jpeg.jpg",//task.Result.ImageUrl.AbsoluteUri
            Image = "AllosTest",//task.Result.ImageUrl.AbsoluteUri
        };

        RunJobStart(request);
    }

    IEnumerator LoadProfilePics()
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError
                || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                isImageLoadingStopped = true;
                UserDetail.IsImageLoadingStopped = isImageLoadingStopped;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                imageSprite = SpriteFromTexture2D(texture);
                UserDetail.Sprite = imageSprite;
            }
        }
    }

    public void OnSignOut()
    {
        //GoogleSignIn.DefaultInstance.SignOut();
        /*authPanel.SetActive(false);
        homeView.SetActive(true);*/
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }
    private void OnApplicationQuit()
    {
        //OnSignOut();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                }
            }
        }
        else if (task.IsCanceled) { }
        else
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

            RunJobStart(request);
        }
    }

    public void RunJobStart(UserDetailDto request)
    {
        UserDetail = request;

        imageUrl = request.Image;

        StartCoroutine(LoadProfilePics());

        _ = Task.Run(() => ExternalService.Login(request));

        _ = Task.Run(async () =>
        {
            var data = await ExternalService.FetchUserGamingCount(request.UserId);
            GamingCount = data;
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

            var incomingTopics = await ExternalService.FetchAvailableTopics(UserDetail.UserId);

            if (hasSavedTopic)
            {
                MainThreadDispatcher.Enqueue(() =>
                {
                    PlayerPrefExtension<List<TopicResponseDto>>.UpdateDb(incomingTopics);
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
        authPanel.SetActive(false);
        pages.SetActive(true);
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