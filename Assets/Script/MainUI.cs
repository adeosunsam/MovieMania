using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerScoreBoard, opponentScoreBoard, QuestionCountDown, waitingForOpponent;

    [SerializeField]
    private Image playerScoreLineImage, opponentScoreLineImage, countDownImage, waitingLoadingFiller;

    [SerializeField]
    private GameObject inplayPanelView, inplayPanelLoading;

    [SerializeField]
    private Animator loadingCircleAnimator;

    internal float playerScore;
    internal int opponentScore;
    private float questionTimerMax = 10f, questionTimer;

    private readonly float maxScore = 160.0f;

    internal bool finishedTestWaitingOpponent = default, opponentJoined, isGameStarted;

    public event EventHandler OnTimerCountdown;

    public static MainUI Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        playerScoreLineImage.fillAmount = 0f;

        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    internal void LoadWaitingText()
    {
        if (opponentJoined)
        {
            waitingForOpponent.SetText("Loading Questions");
            return;
        }
        waitingForOpponent.SetText("Waiting for Opponent");
    }

    private void LoadUIProgessForWaiting()
    {
        if (waitingLoadingFiller.fillAmount <= 0f
            || waitingLoadingFiller.fillOrigin == (int)OriginHorizontal.Left)
        {
            waitingLoadingFiller.fillOrigin = (int)OriginHorizontal.Left;
            waitingLoadingFiller.fillAmount += Time.deltaTime * .3f;
        }

        if (waitingLoadingFiller.fillAmount >= 1f
            || waitingLoadingFiller.fillOrigin == (int)OriginHorizontal.Right)
        {
            waitingLoadingFiller.fillOrigin = (int)OriginHorizontal.Right;
            waitingLoadingFiller.fillAmount -= Time.deltaTime * .3f;

        }
    }

    private void ResetWaitingFiller()
    {
        waitingLoadingFiller.fillOrigin = (int)OriginHorizontal.Left;
        waitingLoadingFiller.fillAmount = 0f;
    }

    internal void StartGame()
    {
        finishedTestWaitingOpponent = true;
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
        inplayPanelLoading.SetActive(false);
        inplayPanelView.SetActive(true);
        ResetWaitingFiller();
        Singleton.opponentJoined = false;
    }

    private void Update()
    {
        if (isGameStarted)
            QuestionTimer();

        if (playerScoreLineImage.fillAmount < (playerScore / maxScore))
        {
            var amountToFill = Math.Clamp(Time.deltaTime * .3f, 0f, 1f / 8f);
            playerScoreLineImage.fillAmount += amountToFill;
        }

        if (opponentScoreLineImage.fillAmount < (opponentScore / maxScore))
        {
            var amountToFill = Math.Clamp(Time.deltaTime * .3f, 0f, 1f / 8f);
            opponentScoreLineImage.fillAmount += amountToFill;
        }

        UpdateScoreClient();

        LoadWaitingText();
    }

    private void LateUpdate()
    {
        if (!finishedTestWaitingOpponent)
        {
            LoadUIProgessForWaiting();
        }
    }

    internal void UpdateScoreServer(int? playerScore = null)
    {
        if (playerScore != null)
            this.playerScore = playerScore.Value;

        string scoreText = $"{this.playerScore}";

        this.playerScoreBoard.SetText(scoreText);
    }

    internal void UpdateScoreClient(int? playerScore = null)
    {
        if (playerScore != null)
            opponentScore = playerScore.Value;

        string scoreText = $"{opponentScore}";

        opponentScoreBoard.SetText(scoreText);
    }

    internal void QuestionTimer()
    {
        QuestionCountDown.SetText($"{(int)questionTimerMax}");

        questionTimer += Time.deltaTime * .7f;

        if (questionTimer >= 1f)
        {
            questionTimer = 0f;
            questionTimerMax -= 1f;
        }

        if (questionTimerMax > 0f)
        {
            return;
        }
        //This is required to display the current timer when timeOut. WHEN TIMER IS 0.
        QuestionCountDown.SetText($"{(int)questionTimerMax}");

        OnTimerCountdown?.Invoke(this, EventArgs.Empty);
    }

    internal void ResetTimer()
    {
        questionTimerMax = 10f;
        QuestionCountDown.SetText($"{(int)questionTimerMax}");
    }

    internal void PlayerScore()
    {
        int score;
        playerScore += questionTimerMax * 2f;
        score = (int)playerScore;
        UpdateScoreServer(score);
        BroadcastService.Singleton.UpdateScore(score);
    }
}
