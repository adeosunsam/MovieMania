using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private string recieverEmail;

    [SerializeField]
    private TextMeshProUGUI Score1;

    [SerializeField]
    private TextMeshProUGUI Score2;

    [SerializeField]
    private TextMeshProUGUI QuestionCountDown;

    [SerializeField]
    private Button questionBtn;

    [SerializeField]
    private GameObject questionPanel;

    //private List<ulong> ConnectedClientId;

    [SerializeField]
    private Button clientBtn;
    [SerializeField]
    private Button hostBtn;


    internal int score1;
    internal int score2;
    private float questionTimerMax = 20f;
    private float questionTimer;

    public bool isGameStarted;

    internal bool isClient;

    public event EventHandler OnTimerCountdown;

    public static MainUI Singleton { get; private set; }

    private void Awake()
    {
        Singleton = this;

        clientBtn.onClick.AddListener(async () =>
        {
            BroadcastService.Singleton.Authenticate();

            /*isClient = true;
            await TestLobby.Instance.StartClientWithRelay();*/
        });

        hostBtn.onClick.AddListener(async () =>
        {
            BroadcastService.Singleton.Authenticate();
            //await TestLobby.Instance.StartHostWithRelay();
        });

        /*questionBtn.onClick.AddListener(() =>
        {
            PlayerNetwork.Instance.TestServerRpc(isClient: isClient);
            //TestLobby.Instance.CreateLobby();
        });*/
    }

    internal void StartGame()
    {
        questionPanel.SetActive(true);
        //StartCoroutine(DelayRoutine());
        QuestionManager.Singleton.MapQuestionToUI();
        isGameStarted = true;
    }

    private void Update()
    {
        /*if (isGameStarted)
            QuestionTimer();*/
        UpdateScoreClient();
    }

    internal void UpdateScoreServer(int? playerScore = null)
    {
        if (playerScore != null)
            score1 = playerScore.Value;

        string scoreText = $"Score: {score1}";

        Score1.SetText(scoreText);
    }

    internal void UpdateScoreClient(int? playerScore = null)
    {
        if (playerScore != null)
            score2 = playerScore.Value;

        string scoreText = $"Score: {score2}";

        Score2.SetText(scoreText);
    }

    internal void QuestionTimer()
    {
        QuestionCountDown.SetText($"{questionTimerMax}");

        questionTimer += Time.deltaTime;

        if (questionTimer >= 1f)
        {
            questionTimer = 0f;
            questionTimerMax -= 1f;
        }

        if (questionTimerMax > 0f)
        {
            return;
        }

        OnTimerCountdown?.Invoke(this, EventArgs.Empty);

        questionTimerMax = 20f;
    }

    /*internal void Test()
    {
        if (isClient)
        {
            score2 += (int)questionTimerMax;
        }
        else
        {
            score1 += (int)questionTimerMax;
        }

        //PlayerNetwork.Instance.TestServerRpc(isClient: isClient);
        //PlayerNetwork.Instance.UpdateScore(PlayerScore(), isClient);
    }*/
    internal void PlayerScore()
    {
        int score;
        /*if (isClient)
        {
            score2 += (int)questionTimerMax;
            score = score2;
            Debug.Log($"22--------------------score2: {score2}");
        }
        else
        {
            score1 += (int)questionTimerMax;
            score = score1;
        }*/

        score1 += (int)questionTimerMax;
        score = score1;
        UpdateScoreServer(score);
        BroadcastService.Singleton.UpdateScore(score, recieverEmail);
        //PlayerNetwork.Instance.UpdateScore(score, isClient);
    }
}
