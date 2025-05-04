using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using static SharedResources;

public class BroadcastService : MonoBehaviour
{
    [SerializeField]
    private string opponentId;

    private HubConnection hubconnection;
    private string url = "https://oluwakemi-001-site1.jtempurl.com/chatHub";
    //private string url = "http://localhost:5060/chatHub";

    private string groupId;

    private string topicId { get; set; }

    public static BroadcastService Singleton { get; private set; }

    public bool OpponentGameOver { get; set; }

    private void Awake()
    {
        Singleton = this;
    }

    public async void ConnectUser()
    {
        var token = await AuthenticateUser();
        Debug.Log("token" + token);

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
                    x.Sprite = await LoadImageAsync(x.UserImage);
                });
            });
        });

        hubconnection.On("RecieveScore", (int opponentScore) =>
        {
            Debug.Log($"Score Recieved: {opponentScore}");

            MainUI.Singleton.UpdateScoreClient(opponentScore);
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

        hubconnection.On("ReceiveConnection", () =>
        {
            Debug.Log($"Connection Recieved: BOTH");
            MainUI.Singleton.opponentJoined = true;

            _ = Task.Run(async () =>
            {
                TopicInPlay = TopicResponse.FirstOrDefault(x => x.Id == topicId);
                var data = await ExternalService.GetQuestionByTopic(topicId);
                Questions = data;
                StartGameInMainThread();
            });

        });

        hubconnection.Closed += async (error) =>
        {
            Debug.Log("Connection closed. Attempting to reconnect...");

            await Task.Delay(2000);

            try
            {
                await hubconnection.StartAsync();
                Debug.Log("Hub connection successfully restarted.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error restarting hub connection: " + ex.GetBaseException().Message);
            }
        };

        hubconnection.StartAsync().ContinueWith(task =>
        {
            Debug.Log("Hub connection attempt finished.");
            if (task.IsFaulted || !task.IsCompletedSuccessfully)
            {
                Debug.LogError("Error starting connection: " + task.Exception?.GetBaseException().Message);
            }
            else if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"HUBCONNECTION STARTED");

                //_ = Task.Run(async () =>
                //{
                //    OpponentDetail.Sprite = await LoadImageAsync(OpponentDetail.Image);
                //});
            }
            else
            {
                Debug.LogError($"ERROR ON CONNECTION: {task.Exception?.GetBaseException().Message}");
            }
        });

        //_ = Task.Run(() =>
        //{
        //    hubconnection = new HubConnectionBuilder().WithUrl(url, options =>
        //    {
        //        options.AccessTokenProvider = () => Task.FromResult(token);
        //    }).Build();

        //    hubconnection.On("RecieveNotification", () =>
        //    {
        //        Debug.Log($"Notification Recieved");

        //        _ = Task.Run(async () =>
        //        {
        //            var data = await ExternalService.GetUserActivity(UserDetail.UserId);
        //            UserActivity = data;
        //            ActivityPage.Singleton.hasActivityUpdate = true;
        //            UserActivity.ForEach(async x =>
        //            {
        //                x.Sprite = await LoadImageAsync(x.UserImage);
        //            });
        //        });
        //    });

        //    hubconnection.On("RecieveScore", (int opponentScore) =>
        //    {
        //        Debug.Log($"Score Recieved: {opponentScore}");

        //        MainUI.Singleton.UpdateScoreClient(opponentScore);
        //    });

        //    hubconnection.On("ReceiveMessage", (string message) =>
        //    {
        //        Debug.Log($"Message Recieved: {message}");
        //    });

        //    hubconnection.On("GameOverNotification", () =>
        //    {
        //        Debug.Log($"Gameover notification Recieved");

        //        ///Stop loading animator and proceed to display scorecard.
        //        OpponentGameOver = true;
        //        //GameOverSection.Singleton.gameEnded = true;
        //    });

        //    hubconnection.On("ReceiveConnection", () =>
        //    {
        //        Debug.Log($"Connection Recieved: BOTH");
        //        MainUI.Singleton.opponentJoined = true;

        //        _ = Task.Run(async () =>
        //        {
        //            TopicInPlay = TopicResponse.FirstOrDefault(x => x.Id == topicId);
        //            var data = await ExternalService.GetQuestionByTopic(topicId);
        //            Questions = data;
        //            StartGameInMainThread();
        //        });

        //    });

        //    Debug.Log("Attempting to start the connection...");

        //    hubconnection.StartAsync().ContinueWith(task =>
        //    {
        //        Debug.Log("Hub connection attempt finished.");
        //        if (task.IsFaulted || !task.IsCompletedSuccessfully)
        //        {
        //            Debug.LogError("Error starting connection: " + task.Exception?.GetBaseException().Message);
        //        }
        //        else if (task.IsCompletedSuccessfully)
        //        {
        //            Debug.Log($"HUBCONNECTION STARTED");

        //            //_ = Task.Run(async () =>
        //            //{
        //            //    OpponentDetail.Sprite = await LoadImageAsync(OpponentDetail.Image);
        //            //});
        //        }
        //        else
        //        {
        //            Debug.LogError($"ERROR ON CONNECTION: {task.Exception?.GetBaseException().Message}");
        //        }
        //    });
        //}); 


        //try
        //{

        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("Error on connection start:   " + ex.Message);
        //    throw;
        //}
    }

    public void PlayGame(string topicId)
    {
        this.topicId = topicId;

        groupId = Guid.NewGuid().ToString();

        CreateGroupAsync(topicId);
    }

    public void JoinAndPlay(string activityId, string topicId)
    {
        this.topicId = topicId;

        var userActivity = UserActivity.First(x => x.Id == activityId);

        groupId = userActivity.GroupId;

        OpponentDetail = UserFriends.First(x => x.UserId == userActivity.ChallengerId);

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
        Debug.Log($"CREATING GROUP WITH OPPONENT: {OpponentDetail.UserId}");
        hubconnection.InvokeAsync("CreateGroupAsync", OpponentDetail.UserId, topicId, groupId);
    }

    public void JoinGroupAsync(string activityId)
    {
        hubconnection.InvokeAsync("JoinGroupAsync", activityId);
    }

    public void OnGameOver()
    {
        Debug.Log("GAME OVER INVOKED");
        hubconnection.InvokeAsync("OnGameFinished", groupId);
    }

    private async Task<string> AuthenticateUser()
    {
        /*if (UserDetail == null)
        {
            return null;
        }*/
        try
        {
            HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var tokenResponse = await HttpClientHelper.PostAsync<TokenResponse>($"api/login?userId={UserDetail.UserId}", content);

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

//public static class UnityWebRequestExtensions
//{
//    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
//    {
//        var tcs = new TaskCompletionSource<UnityWebRequest>();

//        request.SendWebRequest().completed += _ =>
//        {
//            switch (request.result)
//            {
//                case UnityWebRequest.Result.ConnectionError:
//                case UnityWebRequest.Result.ProtocolError:
//                    tcs.TrySetException(new Exception(request.error));
//                    break;
//                default:
//                    tcs.TrySetResult(request);
//                    break;
//            }
//        };
//        return tcs.Task;
//    }
//}

[Serializable]
public class TokenResponse
{
    public string token;
}
