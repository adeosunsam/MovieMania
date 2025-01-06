using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestionDto;

public class InPlayMode : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questionText;

    [SerializeField]
    private Animator titleAnimator;

    [SerializeField]
    public GameObject roundOverlay, sectionPrefab, gameOverPanel;

    [SerializeField]
    private Transform sectionContentContainer;

    internal int TITLE_ANIM_PARAM;

    internal bool mapQuestion;

    private float delayTimer = 2f;

    private Transform correctOptionTransform;
    private static Color missedOption, correctOption;

    public static InPlayMode Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        ColorUtility.TryParseHtmlString("#7f3a4b", out missedOption);
        ColorUtility.TryParseHtmlString("#75bd56", out correctOption);
    }

    void Start()
    {
        TITLE_ANIM_PARAM = Animator.StringToHash("TitleTrigger");

        MainUI.Singleton.OnTimerCountdown += Question_OnTimerCountdown;
    }

    private void Update()
    {
        if (mapQuestion)
            MapQuestionToUI();
    }

    private void Question_OnTimerCountdown(object sender, System.EventArgs e)
    {
        //disable all button
        sectionContentContainer.GetComponentsInChildren<Button>().ToList().ForEach(x => x.interactable = false);
        
        UpdateButtonColor(new Dictionary<GameObject, Color>
        {
            { correctOptionTransform.gameObject, correctOption }
        });

        StartCoroutine(DelayOtherOptionFromDisapearing(new List<Transform> { correctOptionTransform }));
    }

    IEnumerator TitleRoutine(Question question)
    {
        questionText.SetText(question.Title);

        titleAnimator.SetTrigger(TITLE_ANIM_PARAM);

        yield return new WaitForSeconds(2f);

        CountinueExecution(question);

        yield return new WaitForSeconds(.5f);

        //only start countdown after displaying option
        MainUI.Singleton.isGameStarted = true;
    }

    void CountinueExecution(Question question)
    {
        foreach (var option in question.Options)
        {
            var item_go = Instantiate(sectionPrefab);

            item_go.transform.SetParent(sectionContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var textMesh = item_go.GetComponentInChildren<TextMeshProUGUI>();

            textMesh.text = option.Title;

            // Add an onClick listener to the button component
            bool hasButton = item_go.TryGetComponent<Button>(out var button);

            if (hasButton)
            {
                if (option.IsCorrectOption)
                {
                    correctOptionTransform = item_go.transform;
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    Dictionary<GameObject, Color> objectToUpdate = new();

                    List<Transform> skipOption = new();

                    sectionContentContainer.GetComponentsInChildren<Button>().ToList().ForEach(x => x.interactable = false);

                    skipOption.Add(item_go.transform);

                    if (option.IsCorrectOption)
                    {
                        objectToUpdate.Add(correctOptionTransform.gameObject, correctOption);
                        MainUI.Singleton.PlayerScore();
                    }
                    else if (!option.IsCorrectOption)
                    {
                        skipOption.Add(correctOptionTransform);

                        objectToUpdate.Add(button.gameObject, missedOption);
                        objectToUpdate.Add(correctOptionTransform.gameObject, correctOption);
                    }

                    UpdateButtonColor(objectToUpdate);

                    StartCoroutine(DelayOtherOptionFromDisapearing(skipOption));
                });
            }
        }
    }

    private IEnumerator DelayOtherOptionFromDisapearing(List<Transform> skipOption)
    {
        yield return new WaitForSeconds(1f);
        DarkOutTheOtherOption(sectionContentContainer.childCount, sectionContentContainer, skipOption);
        MoveToNexQuestion();
    }

    public void DarkOutTheOtherOption(int childCount, Transform parent, List<Transform> skipOption)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (skipOption.Contains(child))
                continue;

            var image = child.gameObject.GetComponent<Image>();

            if (image)
            {
                Color color = image.color;
                color.a = 0f; // Set alpha to 0
                image.color = color;
            }
            child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
        }
    }

    public void RefreshAvailableLevels(int childCount, Transform parent)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            Destroy(child.gameObject);
        }
    }

    private void UpdateButtonColor(Dictionary<GameObject, Color> objectToUpdate)
    {
        foreach (var item in objectToUpdate)
        {
            item.Key.GetComponent<Image>().color = item.Value;
            item.Key.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
    }

    private void MapQuestionToUI()
    {
        var questions = SharedResources.Questions;
        int currentQuestion = QuestionManager.Singleton.currentQuestion;

        var question = questions.Single(x => x.IndexNumber == currentQuestion);

        StartCoroutine(TitleRoutine(question));
        mapQuestion = false;
    }

    private void MoveToNexQuestion()
    {
        MainUI.Singleton.isGameStarted = false;

        //reset correct option button
        correctOptionTransform = null;

        var nextQuestionNumber = QuestionManager.Singleton.currentQuestion + 1;

        if(nextQuestionNumber > SharedResources.Questions.Count)
        {
            ///no more question
            ///reset the currentQuestion to 0 and display the loading animator
            QuestionManager.Singleton.currentQuestion = 1;

            //MainUI.Singleton.isGameStarted = false;

            // Start the delay coroutine
            StartCoroutine(DelayAndProceedLastQuestionRoutine());
        }
        else
        {
            QuestionManager.Singleton.currentQuestion = nextQuestionNumber;

            //MainUI.Singleton.isGameStarted = false;

            // Start the delay coroutine
            StartCoroutine(DelayAndProceedRoutine());
        }
        /*QuestionManager.Singleton.currentQuestion = Mathf.Clamp(QuestionManager.Singleton.currentQuestion + 1, 1, QuestionManager.Singleton.questions.Count);

        if()*/

        
    }

    private IEnumerator DelayAndProceedLastQuestionRoutine()
    {
        //delay the wrong and right from disappearing immediately
        while (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            yield return null;
        }

        delayTimer = 2f;

        //MainUI.Singleton.ResetTimer();
        //questionText.SetText(string.Empty);
        //NextRoundOverlay.Instance.MapNextRoundData();
        //roundOverlay.SetActive(true);
        //NextRoundOverlay.Instance.showOverlay = true;

        int childCount = sectionContentContainer.childCount;
        RefreshAvailableLevels(childCount, sectionContentContainer);

        MainUI.Singleton.ResetTimer();
        questionText.SetText(string.Empty);

        gameObject.SetActive(false);

        //display the animator
        gameOverPanel.SetActive(true);

        ///sends a notification over to the opponent notifying them that you are done playing
        ///and they are good to go to display your score.
        BroadcastService.Singleton.OnGameOver();
    }

    private IEnumerator DelayAndProceedRoutine()
    {
        //delay the wrong and right from disappearing immediately
        while (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
            yield return null;
        }

        delayTimer = 2f;

        //MainUI.Singleton.ResetTimer();
        //questionText.SetText(string.Empty);
        NextRoundOverlay.Instance.MapNextRoundData();
        roundOverlay.SetActive(true);
        NextRoundOverlay.Instance.showOverlay = true;

        MainUI.Singleton.ResetTimer();
        questionText.SetText(string.Empty);

        int childCount = sectionContentContainer.childCount;
        RefreshAvailableLevels(childCount, sectionContentContainer);

        gameObject.SetActive(false);
    }
}
