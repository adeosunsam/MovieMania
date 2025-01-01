using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

using static SharedResources;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]
    private Image resultPanelCountDown;

    [SerializeField]
    private GameObject gameOverPanel, pages, nextRoundOverlay, inplayViews, inplayPanel, inplayLoading, gameOverLoading;

    public event EventHandler OnTimerCountdown;

    public static ResultPanel Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        resultPanelCountDown.fillAmount = 1f;
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
        if(resultPanelCountDown.fillAmount <= 0f)
        {
            Debug.LogWarning($"RESULT PANEL STILL RUNNING WITH FILLAMOUNT == {resultPanelCountDown.fillAmount}");

            //reset fillAmount

            //gameObject.SetActive(false);

            //OnTimerCountdown?.Invoke(this, EventArgs.Empty);
            Reset();
        }
    }

    void Reset()
    {
        resultPanelCountDown.fillAmount = 1f;

        gameObject.SetActive(false);

        ///activate both loading
        ///activate nextRoundOverlay
        ///deactivate inplayViews, then inplayPanel
        ///deactivate gameOverPanel
        ///

        //refresh the challenges
        _ = Task.Run(async () =>
        {
            var data = await ExternalService.GetUserActivity(UserDetail.UserId);
            UserActivity = data;
            ActivityPage.Singleton.hasActivityUpdate = true;
            UserActivity.ForEach(async x =>
            {
                x.Sprite = await LoadTopicImageAsync(x.UserImage);
            });
        });

        MainUI.Singleton.ResetScore();

        inplayLoading.SetActive(true);
        gameOverLoading.SetActive(true);
        nextRoundOverlay.SetActive(true);
        inplayViews.SetActive(false);
        inplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pages.SetActive(true);
    }

    /*internal void QuestionTimer()
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
    }*/
}
