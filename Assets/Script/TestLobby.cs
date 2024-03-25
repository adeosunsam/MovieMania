using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


public class TestLobby : NetworkBehaviour
{
    [SerializeField]
    private string lobbyCode;

    /*[SerializeField]
    private string joinCode;*/

    public static TestLobby Instance;

    private float heartBeatTimer;

    private Lobby lobby;

    //private string relayCode;

    /*private NetworkVariable<LobbyData> lobbyData = new(new LobbyData
    {
        hostLobbyId = string.Empty,
        //joinCode = string.Empty
    },
        writePerm: NetworkVariableWritePermission.Owner);

    public struct LobbyData : INetworkSerializable, IEquatable<LobbyData>
    {
        public FixedString64Bytes hostLobbyId;
        //public FixedString32Bytes joinCode;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref hostLobbyId);
            //serializer.SerializeValue(ref joinCode);
        }
        public bool Equals(LobbyData other)
        {
            return hostLobbyId == other.hostLobbyId;// && joinCode == other.joinCode;
        }
    }*/

    private void Awake()
    {
        Instance = this;
    }
    /*private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"SignedIn PlayerId: {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }*/
    public override void OnNetworkSpawn()//use this instead of start.
    {
        //NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

        //lobbyData.OnValueChanged += OnStateChanged;
    }

    /*private void NetworkManager_OnClientConnectedCallback(ulong id)
    {
        Debug.Log($"SUCCESSFULLY CONNECTED TO CLIENT: {id}");
        if (id > 1)
        {
            Debug.Log($"CONNECTED CLIENT ID: {id}");

            if (lobby != null)
                //i'm not sure this will work, no client has been connected at this point and sending data to free air might break.
                lobbyData.Value = new LobbyData
                {
                    hostLobbyId = lobby.Id,
                    //joinCode = joinCode
                };
        }
    }*/

    /*public async void OnStateChanged(LobbyData previousValue, LobbyData newValue)
    {
        try
        {
            lobby = await Lobbies.Instance.GetLobbyAsync(newValue.hostLobbyId.ToString());
            //relayCode = newValue.joinCode.ToString();

            Debug.Log($"LobbyName: {lobby.Name}; AvailableSlot: {lobby.AvailableSlots}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }*/

    /*async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"SignedIn PlayerId: {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }*/

    /*public IEnumerator StartHostRoutine()
    {
        var serverRelayUtilityTask = AllocateRelayServiceAndGetJoinCode(2);
        while(!serverRelayUtilityTask.IsCompleted)
        {
            yield return null;
        }
        if(serverRelayUtilityTask.IsFaulted)
        {
            yield break;
        }

        var (ipv4Address, port, allocationIdBytes, key, connectionData, joinCode) = serverRelayUtilityTask.Result;

        print("join code to user");

        CreateLobby(joinCode);

        NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(ipv4Address, port, allocationIdBytes, key, connectionData);
        NetworkManager.StartServer();

        //i'm not sure this will work, no client has been connected at this point and sending data to free air might break.
        lobbyData.Value = new LobbyData
        {
            hostLobbyId = lobby.Id
        };

        yield return null;
    }*/
    /*public static async Task<(string ipv4Address, ushort port, byte[] allocationIdBytes, byte[] key, byte[] connectionData, string joinCode)> AllocateRelayServiceAndGetJoinCode(int maxConnection, string region = null)
    {
        Allocation allocation;
        string createJoinCode;

        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnection, region);
        }
        catch (Exception ex)
        {
            Debug.LogError($"relay create allocation request failed {ex.Message}");
            throw;
        }

        try
        {
            createJoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"relay join code request failed {ex.Message}");
            throw;
        }

        return (allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, createJoinCode);
    }*/

    public async Task<string> StartHostWithRelay(int maxConnections = 2)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        await CreateLobby(joinCode);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async Task<bool> StartClientWithRelay()
    {
        await UnityServices.InitializeAsync();

#if UNITY_EDITOR
        AuthenticationService.Instance.ClearSessionToken();
#endif
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //Join Lobby
        await JoinLobby();

        // get the relay code
        var joinCode = lobby.Data["RelayCode"].Value;

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
    private void Update()
    {
        HandleLobbyHeartBeat();
    }

    private async void HandleLobbyHeartBeat()
    {
        if (lobby != null  && !MainUI.Singleton.isClient)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            }
        }
    }

    internal async Task CreateLobby(string joinCode)
    {
        try
        {
            string lobbyName = "My Lob";
            int maxPlayer = 2;

            var option = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "RelayCode", new DataObject(
                        DataObject.VisibilityOptions.Member,
                        value: joinCode,
                        index: DataObject.IndexOptions.S1
                        )
                    }
                }
            };

            var result = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, option);

            lobby = result;

            Debug.Log($"Created Lobby: {result.Id}; MaxMember: {maxPlayer}");
            Debug.Log($"Created Lobby CODE: {lobby.LobbyCode}");
            //Debug.Log($"Created Join CODE: {joinCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }

    internal async Task JoinLobby()
    {
        try
        {
            var result = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            lobby = result;
            // get the relay code
            //var relayCode = lobby.Data["RelayCode"].Value;
            /*//start client
            var startClient = await StartClientWithRelay(relayCode);
            if (!startClient)
            {

            lobby = result;;
            }*/
            Debug.Log($"LobbyName: {lobby.Name}; AvailableSlot: {lobby.AvailableSlots}");

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e.Message);
        }
        /*catch (Exception e)
        {
            Debug.LogError(e.Message);
        }*/
    }

    private void OnApplicationQuit()
    {
        if(lobby != null)
        Lobbies.Instance.DeleteLobbyAsync(lobby.Id);
    }
}
