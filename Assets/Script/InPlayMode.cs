using System.Collections;
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
    private Animator questionAnimator, titleAnimator;

    [SerializeField]
    private QuestionOption[] optionObject;

    [SerializeField]
    public GameObject roundOverlay;

    internal int IS_PLAYING_ANIM_PARAM;
    internal int TITLE_ANIM_PARAM;

    internal bool mapQuestion;

    public static InPlayMode Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        IS_PLAYING_ANIM_PARAM = Animator.StringToHash("OptionTrigger");
        TITLE_ANIM_PARAM = Animator.StringToHash("TitleTrigger");

        MainUI.Singleton.OnTimerCountdown += Question_OnTimerCountdown;
    }

    private void Update()
    {
        if(mapQuestion)
            MapQuestionToUI();
    }

    private void Question_OnTimerCountdown(object sender, System.EventArgs e)
    {
        MoveToNexQuestion();
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
        int questionOptionIndex = 0;

        foreach (var option in question.Options)
        {
            questionOptionIndex++;
            var optionItem = optionObject.FirstOrDefault(x => x.index == questionOptionIndex);

            if (optionItem == null)
            {
                Debug.LogError($"No matching element found for index: {questionOptionIndex}");
                continue; // Skip to the next iteration if no matching item is found
            }

            var item_go = optionItem.option;

            item_go.SetActive(true);

            item_go.GetComponentInChildren<TextMeshProUGUI>().text = option.Title;

            // Add an onClick listener to the button component
            Button button = item_go.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    MainUI.Singleton.PlayerScore();
                    MainUI.Singleton.ResetTimer();

                    MoveToNexQuestion();
                });
            }
        }

        questionAnimator.SetTrigger(IS_PLAYING_ANIM_PARAM);
    }

    public void MapQuestionToUI()
    {
        var questions = QuestionManager.Singleton.questions;
        int currentQuestion = QuestionManager.Singleton.currentQuestion;

        var question = questions.Single(x => x.IndexNumber == currentQuestion);

        StartCoroutine(TitleRoutine(question));
        mapQuestion = false;
    }

    private void MoveToNexQuestion()
    {
        QuestionManager.Singleton.currentQuestion = Mathf.Clamp(QuestionManager.Singleton.currentQuestion + 1, 1, QuestionManager.Singleton.questions.Count);
        //MoveToNexQuestion();
        //trigger a courotine to wait for 2sec before moving, and disable all button

        questionText.SetText(string.Empty);
        roundOverlay.SetActive(true);
        NextRoundOverlay.Instance.showOverlay = true;

        MainUI.Singleton.isGameStarted = false;

        optionObject.ToList().ForEach(x =>
        {
            x.option.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            x.option.SetActive(false);
        });

        gameObject.SetActive(false);
    }

}
