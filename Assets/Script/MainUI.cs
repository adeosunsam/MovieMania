using System;
using System.Collections;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    /*[SerializeField]
    private string recieverUserId;
*/
    [SerializeField]
    private TextMeshProUGUI playerScore;

    [SerializeField]
    private TextMeshProUGUI opponentScore;

    [SerializeField]
    private TextMeshProUGUI QuestionCountDown;


    [SerializeField]
    private GameObject inplayPanelView, inplayPanelLoading;

    [SerializeField]
    private Image playerScoreLineImage, opponentScoreLineImage, countDownImage;

    [SerializeField]
    private Animator loadingCircleAnimator;

    internal float score1;
    internal int score2;
    private float questionTimerMax = 10f;
    private float questionTimer;

    private float maxScore = 160.0f;
    //private float scoreRemaining;

    public bool isGameStarted;

    internal bool isClient;

    public event EventHandler OnTimerCountdown;

    public static MainUI Singleton { get; private set; }

    private void Awake()
    {
        if(Singleton == null)
            Singleton = this;

        playerScoreLineImage.fillAmount = 0f;

        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    internal void StartGame()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
        inplayPanelLoading.SetActive(false);
        inplayPanelView.SetActive(true);
        QuestionManager.Singleton.MapQuestionToUI();
        isGameStarted = true;
    }

    private void Update()
    {
        if (isGameStarted)
            QuestionTimer();

        if (playerScoreLineImage.fillAmount < (score1 / maxScore))
        {
            var amountToFill = Math.Clamp(Time.deltaTime * .3f, 0f, 1f / 8f);
            playerScoreLineImage.fillAmount += amountToFill;
        }

        UpdateScoreClient();
    }

    internal void UpdateScoreServer(int? playerScore = null)
    {
        if (playerScore != null)
            score1 = playerScore.Value;

        string scoreText = $"{score1}";

        this.playerScore.SetText(scoreText);
    }

    internal void UpdateScoreClient(int? playerScore = null)
    {
        if (playerScore != null)
            score2 = playerScore.Value;

        string scoreText = $"{score2}";

        opponentScore.SetText(scoreText);
    }

    internal void QuestionTimer()
    {
        QuestionCountDown.SetText($"{(int)questionTimerMax}");

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

        ResetTimer();
    }

    internal void ResetTimer()
    {
        questionTimerMax = 10f;
    }

    internal void PlayerScore()
    {
        int score;
        score1 += questionTimerMax * 2f;
        score = (int)score1;
        UpdateScoreServer(score);
        //BroadcastService.Singleton.UpdateScore(score, recieverUserId);
    }
}
