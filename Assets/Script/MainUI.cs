using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI Score1;

    [SerializeField]
    private TextMeshProUGUI Score2;


    [SerializeField]
    private Button questionBtn;

    private List<ulong> ConnectedClientId;

    [SerializeField]
    private Button clientBtn;
    [SerializeField]
    private Button hostBtn;


    internal int score1;
    internal int score2;

    public static MainUI Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            //TestLobby.Instance.JoinLobby();
            //Instantiate(Score1);
            //Instantiate(Score2);
        });

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            /*Instantiate(Score1);
            Score1.gameObject.SetActive(true);*/
            //Instantiate(Score2);
        });

        questionBtn.onClick.AddListener(() =>
        {
            PlayerNetwork.Instance.TestServerRpc();
            //TestLobby.Instance.CreateLobby();
        });
    }

    void Start()
    {
    }

    /*internal void AddListenerToQuestionBtn()
    {
        questionBtn.onClick.AddListener(() =>
        {
            PlayerNetwork.Instance.TestServerRpc();
        });
    }*/

    private void Update()
    {
        /*UpdateScoreServer();
        UpdateScoreClient();*/
    }



    internal void UpdateScoreServer()
    {
        string scoreText = $"Score: {++score1}";
        Score1.text = scoreText;
        //Score1.SetText(scoreText);
    }

    internal void UpdateScoreClient()
    {
        string scoreText = $"Score: {score2}";
        Score2.SetText(scoreText);
    }

    public void QuestionOnClick()
    {

    }

    
}
