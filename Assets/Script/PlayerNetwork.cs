using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    /*[SerializeField] GameObject scorePrefab1;
    [SerializeField] GameObject scorePrefab2;
    [SerializeField] Transform scorePositionServer;
    [SerializeField] Transform scorePositionClient;*/

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

    public struct CustomData : INetworkSerializable
    {
        public int playerScore;
        public string playerName;// use FixedString128Bytes because string is a reference type

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerScore);
            serializer.SerializeValue(ref playerName);
        }
    }

    public override void OnNetworkSpawn()//use this instead of start.
    {
        randomNum.OnValueChanged += (CustomData previousValue, CustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue.playerScore + "; " + newValue.playerName);
            MainUI.Singleton.UpdateScoreServer();
        };
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            randomNum.Value = new CustomData
            {
                playerScore = MainUI.Singleton.score1++,
                playerName = "North London Forever"
            };
        }*/

        //if(!IsHost)
        //TestClientRpc();
        /*else if (IsHost)
            TestServerRpc(new ServerRpcParams());*/
    }


    [ServerRpc(RequireOwnership = false)]
    public void TestServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"TestServerRpc: {OwnerClientId}---------SenderId: {serverRpcParams.Receive.SenderClientId}");
        //TestClientRpc();
        /*MainUI.Singleton.UpdateScoreServer();
        MainUI.Singleton.UpdateScoreClient();*/
        //if(NetworkManager.LocalClientId == )
        TestClientRpc();
        
        //MainUI.Singleton.UpdateScore(OwnerClientId.ToString());
    }

    [ClientRpc]
    public void TestClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("TestClientRpc: " + OwnerClientId);

        /*randomNum.Value = new CustomData
        {
            playerScore = MainUI.Singleton.score1,
            playerName = "North London Forever"
        };*/
        if(NetworkManager.LocalClientId == 1)
        {
            MainUI.Singleton.UpdateScoreClient();
            return;
        }
        MainUI.Singleton.UpdateScoreServer();

        /*if (!Instance)
        {
            if (IsHost)
            {
                Instance = Instantiate(scorePrefab, scorePositionServer);
                //Instance.GetComponent<NetworkObject>().Spawn(true);
            }
            else
            {
                Instance = Instantiate(scorePrefab, scorePositionClient);
                //Instance.GetComponent<NetworkObject>().Spawn(true);
            }
        }
        else
        {
            Instance.GetComponent<TextMeshProUGUI>().text = $"Score: {MainUI.Singleton.score1}";
            if (IsHost)
            {
                scorePrefab.GetComponent<TextMeshProUGUI>().text = $"Score: {MainUI.Singleton.score1}";
            }
            else
            {
                scorePrefab.GetComponent<TextMeshProUGUI>().text = $"Score: {MainUI.Singleton.score2}";
            }
        }*/

        /*if (IsHost)
        {
            MainUI.Singleton.UpdateScoreServer();
        }
        else if(IsClient)
        {
            MainUI.Singleton.UpdateScoreClient();
        }*/


        //MainUI.Singleton.UpdateScoreClient();
        //Debug.Log($"TestClientRpc:   ClientId: {OwnerClientId}");
        //MainUI.Singleton.UpdateScoreClient($"TestClientRpc:   Server: {OwnerClientId}");
        //MainUI.Singleton.UpdateScore(OwnerClientId.ToString());
    }

}
