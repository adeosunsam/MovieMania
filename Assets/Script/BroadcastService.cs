using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static SharedResources;

public class BroadcastService : MonoBehaviour
{
    [SerializeField]
    private string opponentId;

    private HubConnection hubconnection;
    //private string url = "https://odemwingiee-001-site1.ltempurl.com/chatHub";
    private string url = "http://localhost:5060/chatHub";

    private string groupId;

    public static BroadcastService Singleton { get; private set; }

    public bool OpponentGameOver { get; set; }

    private void Awake()
    {
        Singleton = this;
    }

    public async void ConnectUser()
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

        hubconnection.On("RecieveNotification", () =>
        {
            Debug.Log($"Notification Recieved");
            
            _ = Task.Run(async () =>
            {
                var data = await ExternalService.GetUserActivity(UserDetail.UserId);
                UserActivity = data;
                ActivityPage.Singleton.hasActivityUpdate = true;
                UserActivity.ForEach(async x =>
                {
                    x.Sprite = await LoadTopicImageAsync(x.UserImage);
                });
            });
        });

        hubconnection.On("ReceiveMessage", (string message) =>
        {
            Debug.Log($"Message Recieved: {message}");
        });

        hubconnection.On("GameOverNotification", () =>
        {
            Debug.Log($"Gameover notification Recieved");

            ///Stop loading animator and proceed to display scorecard.
            OpponentGameOver = true;
            //GameOverSection.Singleton.gameEnded = true;
        });

        try
        {
            await hubconnection.StartAsync();

            Debug.Log($"HUBCONNECTION STARTED");
        }
        catch (Exception ex)
        {
            Debug.Log("Error on connection start:   " + ex.Message);
            throw;
        }
    }

    public void Authenticate(string topicId)
    {
        hubconnection.On("RecieveScore", (int opponentScore) =>
        {
            Debug.Log($"Score Recieved: {opponentScore}");

            MainUI.Singleton.UpdateScoreClient(opponentScore);
        });

        hubconnection.On("ReceiveConnection", () =>
        {
            Debug.Log($"Connection Recieved: INITIATOR");
            MainUI.Singleton.opponentJoined = true;

            _ = Task.Run(async () =>
            {
                TopicInPlay = TopicResponse.FirstOrDefault(x => x.Id == topicId);
                var data = await ExternalService.GetQuestionByTopic(topicId);
                Questions = data;
                StartGameInMainThread();
            });

        });

        groupId = Guid.NewGuid().ToString();

        CreateGroupAsync(topicId);
    }

    public void JoinAndPlay(string activityId, string topicId)
    {
        hubconnection.On("RecieveScore", (int opponentScore) =>
        {
            Debug.Log($"Score Recieved: {opponentScore}");

            MainUI.Singleton.UpdateScoreClient(opponentScore);
        });

        hubconnection.On("ReceiveConnection", () =>
        {
            Debug.Log($"Connection Recieved: RECIEVER");
            MainUI.Singleton.opponentJoined = true;

            _ = Task.Run(async () =>
            {
                TopicInPlay = TopicResponse.FirstOrDefault(x => x.Id == topicId);
                var data = await ExternalService.GetQuestionByTopic(topicId);
                Questions = data;
                StartGameInMainThread();
            });

        });

        groupId = UserActivity.First(x => x.Id == activityId).GroupId;

        JoinGroupAsync(activityId);
    }

    private void StartGameInMainThread()
    {
        MainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                MainUI.Singleton.StartGame();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        });
    }

    public void UpdateScore(int playerScore)
    {
        hubconnection.InvokeAsync("SendMessageAsync", playerScore, groupId);
    }

    public void CreateGroupAsync(string topicId)
    {
        hubconnection.InvokeAsync("CreateGroupAsync", opponentId, topicId, groupId);
    }

    public void JoinGroupAsync(string activityId)
    {
        hubconnection.InvokeAsync("JoinGroupAsync", activityId);
    }

    public void OnGameOver()
    {
        hubconnection.InvokeAsync("OnGameFinished", groupId);
    }

    private async Task<string> AuthenticateUser()
    {
        /*if (UserDetail == null)
        {
            return null;
        }*/
        // Replace with your authentication endpoint
        //string authUrl = $"https://localhost:7153/api/login?userId={userId}";
        //string authUrl = $"https://leaderboard-o33d.onrender.com/api/login?userId={UserDetail.UserId}";
        /*string authUrl = $"https://odemwingie-001-site1.ktempurl.com/api/login?userId={UserDetail.UserId}";
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
        }*/


        //string jsonPostData = JsonConvert.SerializeObject(request);
        try
        {
            HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var tokenResponse = await HttpClientHelper.PostAsync<TokenResponse>($"login?userId={UserDetail.UserId}", content);

            return tokenResponse?.token;
        }
        catch (Exception ex)
        {
            Debug.LogError("Authentication failed: " + ex.Message);
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
