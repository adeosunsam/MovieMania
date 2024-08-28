using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static SharedResources;

public class BroadcastService : MonoBehaviour
{

    //private static readonly HttpClient client = new HttpClient();

    /*[SerializeField]
    private string userId;//this will be gotten from google oauth*/

    private HubConnection hubconnection;
    private string url = "https://leaderboard-o33d.onrender.com/chatHub";
    //private string url = "http://localhost:5060/chatHub";

    public static BroadcastService Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    public async void Authenticate()
    {
        var token = await AuthenticateUser();

        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("Unable to Authenticate User");
            return;
        }

        hubconnection = new HubConnectionBuilder().WithUrl(url, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(token);
        }).Build();

        hubconnection.On("RecieveScore", (int opponentScore) =>
        {
            Debug.Log($"Score Recieved: {opponentScore}");

            MainUI.Singleton.UpdateScoreClient(opponentScore);
        });

        hubconnection.On<string>("ReceiveConnID", (str) =>
        {
            Debug.Log($"Connection ID Recieved: {str}");
        });

        try
        {
            await hubconnection.StartAsync();

            MainUI.Singleton.StartGame();
        }
        catch (Exception ex)
        {
            Debug.Log("Error on connection start:   " + ex.Message);
            throw;
        }
    }

    public void UpdateScore(int playerScore, string userId)
    {
        hubconnection.InvokeAsync("SendMessageAsync", playerScore, userId);
    }

    private async Task<string> AuthenticateUser()
    {
        if(UserDetail == null)
        {
            return null;
        }
        // Replace with your authentication endpoint
        //string authUrl = $"https://localhost:7153/api/login?userId={userId}";
        string authUrl = $"https://leaderboard-o33d.onrender.com/api/login?userId={UserDetail.UserId}";
        UnityWebRequest www = UnityWebRequest.PostWwwForm(authUrl, "");
        www.SetRequestHeader("Content-Type", "application/json");
        await www.SendWebRequestAsync();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = www.downloadHandler.text;
            var tokenResponse = JsonUtility.FromJson<TokenResponse>(response);
            return tokenResponse.token;
        }
        else
        {
            Debug.LogError("Authentication failed: " + www.error);
            return null;
        }
    }

    private async void OnApplicationQuit()
    {
        if (hubconnection != null)
        {
            await hubconnection.StopAsync();
            await hubconnection.DisposeAsync();
        }
    }
}

public static class UnityWebRequestExtensions
{
    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        request.SendWebRequest().completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                tcs.SetException(new Exception(request.error));
            }
            else
            {
                tcs.SetResult(request);
            }
        };

        return tcs.Task;
    }
}

[Serializable]
public class TokenResponse
{
    public string token;
}
