using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BroadcastService : MonoBehaviour
{

    //private static readonly HttpClient client = new HttpClient();

    [SerializeField]
    private string email;//this will be gotten from google oauth

    private HubConnection hubconnection;
    private string url = "http://localhost:5183/chatHub";

    private string token;

    public static BroadcastService Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    public async void Authenticate()
    {
        token = await AuthenticateUser();

        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("Unable to Authenticate User");
            return;
        }

        hubconnection = new HubConnectionBuilder().WithUrl(url, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(token);
        }).Build();

        hubconnection.On<string>("ReceiveMessage", (string opponentScore) =>
        {
            Debug.Log($"Message Recieved: {opponentScore}");

            MainUI.Singleton.UpdateScoreClient(int.Parse(opponentScore));
        });

        hubconnection.On<string>("ReceiveConnID", (str) =>
        {
            Debug.Log($"Connection ID Recieved: {str}");
        });

        try
        {
            await hubconnection.StartAsync();

            if (!string.IsNullOrEmpty(email))
            {
                MainUI.Singleton.StartGame();
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error on connection start:   " + ex.Message);
            throw;
        }
    }

    public void UpdateScore(int playerScore, string recieverEmail)
    {
        hubconnection.InvokeAsync("SendMessageAsync", playerScore, recieverEmail);
    }

    private async Task<string> AuthenticateUser()
    {
        // Replace with your authentication endpoint
        string authUrl = $"https://localhost:7183/api/login?email={email}";
        UnityWebRequest www = UnityWebRequest.PostWwwForm(authUrl, "");
        www.SetRequestHeader("Content-Type", "application/json");

        // Add user credentials to the request body if needed
        /*var json = "{\"username\":\"user\",\"password\":\"password\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);*/

        await www.SendWebRequestAsync();

        if (www.result == UnityWebRequest.Result.Success)
        {
            var response = www.downloadHandler.text;
            // Parse the token from the response
            var tokenResponse = JsonUtility.FromJson<TokenResponse>(response);
            return tokenResponse.token;
        }
        else
        {
            Debug.LogError("Authentication failed: " + www.error);
            return null;
        }
    }

    /*private async Task<string> AuthenticateUser(string username, string password)
    {
        var credentials = new
        {
            Username = username,
            Password = password
        };

        string json = JsonConvert.SerializeObject(credentials);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("https://yourserver.com/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return tokenResponse.token;
        }
        else
        {
            Debug.LogError($"Authentication failed: {response.StatusCode}");
            return null;
        }
    }*/

    // Update is called once per frame
    void Update()
    {

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
                tcs.SetException(new System.Exception(request.error));
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
