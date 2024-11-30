using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestionDto;
using static ResponseDtos;

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

    internal int OPTION_ANIM_PARAM;
    internal int TITLE_ANIM_PARAM;

    internal bool mapQuestion, updateColor;

    private int? correctOptionIndex;

    public static InPlayMode Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        OPTION_ANIM_PARAM = Animator.StringToHash("OptionTrigger");
        TITLE_ANIM_PARAM = Animator.StringToHash("TitleTrigger");

        MainUI.Singleton.OnTimerCountdown += Question_OnTimerCountdown;
    }

    private void Update()
    {
        if(mapQuestion)
            MapQuestionToUI();

        if (updateColor)
        {
            var correctOption = optionObject.FirstOrDefault(x => x.index == correctOptionIndex);

            if(correctOption != null)
            {
                Debug.LogWarning("Correct Option PRESENT");
                correctOption.option.GetComponent<Image>().color = Color.green;
            }
        }
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

        Debug.Log($"Correct Option Index is: {correctOptionIndex}");

        yield return new WaitForSeconds(.5f);

        //only start countdown after displaying option
        MainUI.Singleton.isGameStarted = true;
    }
/*
    IEnumerator AnswerDelayRoutine()
    {
        yield return new WaitForSeconds(2f);
    }
*/
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

            var textMesh = item_go.GetComponentInChildren<TextMeshProUGUI>();

            textMesh.text = option.Title;

            correctOptionIndex = option.IsCorrectOption ? questionOptionIndex : correctOptionIndex;

            // Add an onClick listener to the button component
            Button button = item_go.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    MainUI.Singleton.PlayerScore();
                    MainUI.Singleton.ResetTimer();

                    {
                        optionObject.ToList().ForEach(x =>
                        {
                            var test = x.option.GetComponent<Button>();

                            test.interactable = false;

                            if(test != button && x.index != correctOptionIndex)
                            {
                                x.option.SetActive(false);
                            }

                            else if(x.index == correctOptionIndex)
                            {
                                updateColor = true;
                                Debug.Log("Corrent Option Hit");
                            }
                        });
                    }
                    //Do courotine and wait 2secs
                    //StartCoroutine(AnswerDelayRoutine());

                    //MoveToNexQuestion();
                });
            }
        }

        questionAnimator.SetTrigger(OPTION_ANIM_PARAM);

        optionObject.ToList().ForEach(x => x.option.GetComponent<Button>().interactable = true);
    }

    private void UpdateButtonColor(Dictionary<GameObject, Color> objectToUpdate)
    {
        foreach(var item in objectToUpdate)
        {
            item.Key.GetComponent<Image>().color = item.Value;
        }
    }

    private void MapQuestionToUI()
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
        
        questionText.SetText(string.Empty);
        NextRoundOverlay.Instance.MapNextRoundData();
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
