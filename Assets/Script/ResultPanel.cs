using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]
    private Image resultPanelCountDown, playerScoreLineImage, opponentScoreLineImage;

    [SerializeField]
    private GameObject gameOverPanel, pages, nextRoundOverlay, inplayViews, inplayPanel, inplayLoading, gameOverLoading;

    [SerializeField]
    private GameObject profilePics, opponentPics;

    [SerializeField]
    private TextMeshProUGUI playerName, opponentName, playerScore, opponentScore, winnerLoserText, currentGameName;

    public event EventHandler OnTimerCountdown;

    public static ResultPanel Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        resultPanelCountDown.fillAmount = 1f;
    }

    private void OnEnable()
    {
        Debug.Log("RESULT PANEL ENABLED");

        var displayImage = profilePics.GetComponent<Image>();
        var opponentImage = opponentPics.GetComponent<Image>();

        if (displayImage && UserDetail.Sprite)
            displayImage.sprite = UserDetail.Sprite;

        if (opponentImage && OpponentDetail.Sprite)
            opponentImage.sprite = OpponentDetail.Sprite;

        LoadTexts();
    }

    private void LoadTexts()
    {
        Debug.Log($"LOAD TEXTS BEGINS: USER-DETAILS IS NULL: {UserDetail == null}, OPPONENT-DETAILS IS NULL: {OpponentDetail == null}");

        playerName.text = $"{UserDetail.FirstName} {UserDetail.LastName}";
        opponentName.text = $"{OpponentDetail.FirstName} {OpponentDetail.LastName}";
        playerScore.text = $"{MainUI.Singleton.playerScore}";
        opponentScore.text = $"{MainUI.Singleton.opponentScore}";
        currentGameName.text = TopicInPlay.Name;

        Debug.Log($"PLAYER NAME: {playerName.text}");
        Debug.Log($"OPPONENT NAME: {opponentName.text}");
        Debug.Log($"PLAYER SCORE: {playerScore.text}");
        Debug.Log($"OPPONENT SCORE: {opponentScore.text}");
        Debug.Log($"CURRENT GAME NAME: {currentGameName.text}");

        if (MainUI.Singleton.playerScore > MainUI.Singleton.opponentScore)
        {
            winnerLoserText.text = "YOU WON!";
            winnerLoserText.color = Color.green;
        }
        else if (MainUI.Singleton.playerScore < MainUI.Singleton.opponentScore)
        {
            winnerLoserText.text = "YOU LOST!";
            winnerLoserText.color = Color.red;
        }
        else
        {
            winnerLoserText.text = "DRAW!";
            winnerLoserText.color = Color.yellow;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (resultPanelCountDown.fillAmount > (playerScore / maxScore))
        {
            var amountToFill = Math.Clamp(Time.deltaTime * .3f, 0f, 1f / 8f);
            resultPanelCountDown.fillAmount += amountToFill;
        }*/
        resultPanelCountDown.fillAmount -= Time.deltaTime * .1f;
        if (resultPanelCountDown.fillAmount <= 0f)
        {
            //OnTimerCountdown?.Invoke(this, EventArgs.Empty);
            Reset();
        }
    }

    void Reset()
    {
        playerScoreLineImage.fillAmount = 0f;
        opponentScoreLineImage.fillAmount = 0f;

        resultPanelCountDown.fillAmount = 1f;

        gameObject.SetActive(false);

        //refresh the challenges
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

        //MainUI.Singleton.ResetScore();

        inplayLoading.SetActive(true);
        gameOverLoading.SetActive(true);
        nextRoundOverlay.SetActive(true);
        inplayViews.SetActive(false);
        inplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pages.SetActive(true);
    }
}
