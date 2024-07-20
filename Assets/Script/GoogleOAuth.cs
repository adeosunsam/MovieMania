using Google;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using static SharedResources;

public class GoogleOAuth : MonoBehaviour
{
    //[SerializeField]
    //private TextMeshProUGUI usernameText, fullname, userEmailText, userId, imageUrl, errorText;

    [SerializeField]
    private GameObject homeView, authPanel, pages;

    private const string webClientId = "1079234788728-hn0tlk70vq570ap3ea4o4j3mqrn268sj.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    //private RestClientHelper _restClient { get; set; }
    private ExternalCallForData External { get; set; }

    /*public GoogleOAuth()
    {
        var provider = GameManager.Instance.Services.BuildServiceProvider();
        _external = provider.GetRequiredService<ExternalCallForData>();
    }*/

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    private string imageUrl;
    private Sprite imageSprite;

    internal bool isImageLoadingStopped;

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        var provider = GameManager.Instance.Services.BuildServiceProvider();
        External = provider.GetRequiredService<ExternalCallForData>();
    }

    public void OnSignIn()
    {
        /*GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished, TaskScheduler.Default);*/

        var request = new UserDetailDto
        {
            FirstName = "Odunayo",// task.Result.DisplayName,
            LastName = "Adeosun",//task.Result.GivenName,
            Email = "adeosunsamsamuel30@gmail.com",//task.Result.Email,
            UserName = "allos",//task.Result.FamilyName,
            UserId = "cb277a70231842a1947596435708e245",//task.Result.UserId,
            Image = "https://res.cloudinary.com/chrismeyer/image/upload/v1652828948/ab71355e-3d40-4ada-95b5-021d08c0e6eeWhatsApp%20Image%202022-03-09%20at%204.37.59%20PM.jpeg.jpg",//task.Result.ImageUrl.AbsoluteUri
        };

        UserDetail = request;

        imageUrl = request.Image;

        StartCoroutine(LoadProfilePics());

        _ = Task.Run(() => External.Login(request));

        _ = Task.Run(async () =>
        {
            //External.Login(request);
            var (IsSuccessful, message, data) = await External.FetchUserGamingCount(request.UserId);
            GamingCount = data;
        });

        _ = Task.Run(async () =>
        {
            TopicResponse = await ExternalService.FetchAvailableTopics(UserDetail.UserId);
            Debug.LogWarning($"TopicResponse COunt:{TopicResponse?.Count}");
            Debug.LogWarning($"TopicResponse: {TopicResponse != null}");
            StartUp();
        });

        authPanel.SetActive(false);
        pages.SetActive(true);
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
        authPanel.SetActive(false);
        homeView.SetActive(true);
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
        else if (task.IsCanceled)
        {
        }
        else
        {
            var request = new UserDetailDto
            {
                FirstName = "Samuel",// task.Result.DisplayName,
                LastName = "Adeosun",//task.Result.GivenName,
                Email = "adeosunsamsamuel30@gmail.com",//task.Result.Email,
                UserName = "allos",//task.Result.FamilyName,
                UserId = $"{Guid.NewGuid():N}",//task.Result.UserId,
                Image = "https://res.cloudinary.com/chrismeyer/image/upload/v1652828948/ab71355e-3d40-4ada-95b5-021d08c0e6eeWhatsApp%20Image%202022-03-09%20at%204.37.59%20PM.jpeg.jpg",//task.Result.ImageUrl.AbsoluteUri
            };

            //_ = Task.Run(async () => await External.Login(request));
        }
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