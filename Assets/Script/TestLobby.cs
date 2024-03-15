using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    public static TestLobby Instance;

    private Lobby hostLobby;
    private float heartBeatTimer;

    private void Awake()
    {
        Instance = this;
    }
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"SignedIn PlayerId: {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Update()
    {
        HandleLobbyHeartBeat();
    }

    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    internal async void CreateLobby()
    {
        try
        {
            string lobbyName = "My Lob";
            int maxPlayer = 2;

            var option = new CreateLobbyOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, $"Samuel{Random.Range(10, 99)}") }
                    }
                }
            };

            var result = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, option);

            hostLobby = result;

            Debug.Log($"Created Lobby: {result.Name}; MaxMember: {maxPlayer}");
        }catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }

    internal async void JoinLobby()
    {
        try
        {
            var lobbies = await Lobbies.Instance.QueryLobbiesAsync();

            var result = await LobbyService.Instance.JoinLobbyByIdAsync(lobbies.Results[0].Id);

            Debug.Log($"LobbyName: {lobbies.Results[0].Name}; AvailableSlot: {lobbies.Results[0].AvailableSlots}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }


    internal async void ListLobbies()
    {
        try
        {
            var filter = new QueryLobbiesOptions
            {
                Count = 2,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots , "1", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(field: QueryOrder.FieldOptions.Created)
                }
            };

            var lobbies = await Lobbies.Instance.QueryLobbiesAsync(filter);
            foreach (var lob in lobbies.Results)
            {
                Debug.Log($"LobbyName: {lob.Name}; AvailableSlot: {lob.AvailableSlots}");
            }
        }catch(LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }
}
