using Google;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoogleOAuth : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI usernameText, fullname, userEmailText, userId, imageUrl, errorText;

    private Image profilePic;

    public string webClientId = "1079234788728-hn0tlk70vq570ap3ea4o4j3mqrn268sj.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished, TaskScheduler.Default);
    }

    public void OnSignOut()
    {
        Debug.LogError("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        Debug.LogError("Calling Disconnect");

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
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    errorText.text = "Got Error: " + error.Status + " " + error.Message;
                }
                else
                {
                    errorText.text = "Got Unexpected Exception?!?" + task.Exception;
                }
            }
        }
        else if (task.IsCanceled)
        {
            errorText.text = "Canceled";
        }
        else
        {
            fullname.text = $"D: {task.Result.DisplayName} G: {task.Result.GivenName}";
            userEmailText.text = $"Email: {task.Result.Email}";
            imageUrl.text = $"Image Url: {task.Result.ImageUrl.AbsoluteUri}";
            usernameText.text = task.Result.FamilyName;
            userId.text = $"UserId: {task.Result.UserId}";
            /*Debug.Log("Display Name: " + task.Result.DisplayName + "!");
            Debug.Log("Email: " + task.Result.Email + "!");
            Debug.Log("Image Url: " + task.Result.ImageUrl + "!");
            Debug.Log("Given Name: " + task.Result.GivenName + "!");
            Debug.Log("Family Name: " + task.Result.FamilyName + "!");
            Debug.Log("UserID: " + task.Result.UserId + "!");
            Debug.Log("Id Token: " + task.Result.IdToken + "!");
            Debug.Log("Auth Code: " + task.Result.AuthCode + "!");*/
        }
    }

    public void OnSignInSilently()
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
    }
}