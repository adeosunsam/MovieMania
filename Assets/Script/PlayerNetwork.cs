using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    public static PlayerNetwork Instance;

    private void Awake()
    {
        Instance = this;
    }

    //Generic can only accept value type, no reference type
    private NetworkVariable<CustomData> randomNum = new(new CustomData
    {
        playerScore = 0,
        playerName = string.Empty
    },
        writePerm: NetworkVariableWritePermission.Owner);

    public struct CustomData : INetworkSerializable, IEquatable<CustomData>
    {
        public int playerScore;
        public FixedString128Bytes playerName;// use FixedString128Bytes because string is a reference type

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                // Now de-serialize the non-complex value type properties
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out playerScore);
                reader.ReadValueSafe(out playerName);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(playerScore);
                writer.WriteValueSafe(playerName);
            }
            // The complex value type handles its own de-serialization
            /*serializer.SerializeValue(ref playerScore);
            serializer.SerializeValue(ref playerName);*/
        }
        public bool Equals(CustomData other)
        {
            return playerScore == other.playerScore && playerName == other.playerName;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOnwerShipServerRpc(ulong ownerId, int score)
    {
        GetComponent<NetworkObject>().ChangeOwnership(ownerId);

        /*randomNum.Value = new CustomData
        {
            playerScore = score,
            playerName = "North London Forever"
        };*/
        OwnerShipGrantedClientRpc(score);
    }

    [ClientRpc]
    public void OwnerShipGrantedClientRpc(int score)
    {
        Debug.LogWarning($"----------OWNERSHIP GRANTED--------ISOWNEDBYSERVER: {IsOwnedByServer};----ISOWNER: {IsOwner};-----ISSERVER{IsServer}");
        // Set the value after ownership is granted on the client side
        if (IsOwner)
        {
            randomNum.Value = new CustomData
            {
                playerScore = score,
                playerName = "North London Forever"
            };
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRebukeOnwerShipServerRpc()
    {
        GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
        //OwnerShipRebukedClientRpc();
    }

    [ClientRpc]
    public void OwnerShipRebukedClientRpc()
    {
        Debug.LogWarning($"----------OWNERSHIP REBUKED--------ISOWNEDBYSERVER: {IsOwnedByServer};----ISOWNER: {IsOwner};-----ISSERVER{IsServer}");

    }

    public override void OnNetworkSpawn()//use this instead of start.
    {
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

        randomNum.OnValueChanged += OnStateChanged;
    }
    public override void OnNetworkDespawn()
    {
        randomNum.OnValueChanged -= OnStateChanged;
    }

    public void OnStateChanged(CustomData previousValue, CustomData newValue)
    {
        //Debug.Log(OwnerClientId + "; " + newValue.playerScore + "; " + newValue.playerName);

        //TestServerRpc(MainUI.Singleton.isClient, newValue.playerScore);

        if ((MainUI.Singleton.isClient && IsOwner) || (!MainUI.Singleton.isClient && !IsOwner))
        {
            MainUI.Singleton.UpdateScoreClient(newValue.playerScore);
        }
        else if ((IsOwner && !MainUI.Singleton.isClient) || (!IsOwner && MainUI.Singleton.isClient))
        {
            MainUI.Singleton.UpdateScoreServer(newValue.playerScore);
        }
    }

    internal void UpdateScore(int score, bool isClient)
    {
        if (!IsOwner && isClient)
        {
            //OwnerShipGrantedClientRpc(score);
            RequestOnwerShipServerRpc(NetworkManager.LocalClientId, score);
            return;
        }
        else if (!IsOwner && !isClient)
        {
            RequestRebukeOnwerShipServerRpc();
        }
        /*if(isClient)
        {
            //
            GetComponent<NetworkObject>().ChangeOwnership(NetworkManager);
        }*/
        randomNum.Value = new CustomData
        {
            playerScore = score,
            playerName = "North London Forever"
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong id)
    {
        Debug.Log($"SUCCESSFULLY CONNECTED TO THE SERVER; Id: {id}");
        if (id == 1)
        {
            MainUI.Singleton.StartGame();
        }
    }

    /*[ServerRpc(RequireOwnership = false)]
    public void TestServerRpc(bool isClient, int score)
    {
        TestClientRpc(isClient: isClient, score);
    }

    [ClientRpc]
    public void TestClientRpc(bool isClient, int score)
    {
        Debug.Log("TestClientRpc: " + OwnerClientId);

        if (isClient)
        {
            MainUI.Singleton.UpdateScoreClient(score);
            return;
        }
        MainUI.Singleton.UpdateScoreServer(score);
    }*/

}
