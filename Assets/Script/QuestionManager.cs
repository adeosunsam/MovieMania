using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestionDto;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questionText;

    [SerializeField]
    private Transform optionParent;

    [SerializeField]
    private Animator questionAnimator;

    [SerializeField]
    private QuestionOption[] optionObject;

    [SerializeField]
    public GameObject roundOverlay, inPlayMode;

    public static QuestionManager Singleton;

    public ICollection<Question> questions;

    public int currentQuestion = 1;

    //private int IS_PLAYING_ANIM_PARAM;
    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //MainUI.Singleton.OnTimerCountdown += Question_OnTimerCountdown;

        //GetQuestion();

        //IS_PLAYING_ANIM_PARAM = Animator.StringToHash("OptionTrigger");
    }

    /*private void Question_OnTimerCountdown(object sender, System.EventArgs e)
    {
        //Move to next question
        MoveToNexQuestion();
    }*/

    // Update is called once per frame
    void Update()
    {

    }

    /*public List<Question> QueryMovieById()
    {
        return new List<Question>
        {
            new Question
            {
                Id = 1,
                Title = "What is Authors second name",
                IndexNumber = 1,
                Options = new List<QuestionDto.QuestionOption>
                {
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "A",
                        Title = "Ayodeji",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "B",
                        Title = "Toluwalope",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "C",
                        Title = "Samuel",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "D",
                        Title = "Tayo",
                        IsCorrectOption = false
                    }
                }
            },
            new Question
            {
                Id= 2,
                Title = "What is the largest west africa city",
                IndexNumber = 2,
                Options = new List<QuestionDto.QuestionOption>
                {
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "A",
                        Title = "Ibadan",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "B",
                        Title = "Abidjan",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "C",
                        Title = "Lagos",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "D",
                        Title = "Kumasi",
                        IsCorrectOption = false
                    }
                }
            },
            new Question
            {
                Id= 3,
                Title = "Who dey?",
                IndexNumber = 3,
                Options = new List<QuestionDto.QuestionOption>
                {
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "A",
                        Title = "Burna Boy",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "B",
                        Title = "Baddo",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "C",
                        Title = "Wizzy",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "D",
                        Title = "O.B.O Baddest",
                        IsCorrectOption = false
                    }
                }
            },
            new Question
            {
                Id= 4,
                Title = "Biggest bird",
                IndexNumber = 4,
                Options = new List<QuestionDto.QuestionOption>
                {
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "A",
                        Title = "Burna Boy",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "B",
                        Title = "Baddo",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "C",
                        Title = "Wizzy",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        //OptionAlphabet = "D",
                        Title = "O.B.O Baddest",
                        IsCorrectOption = false
                    }
                }
            },

        };
    }
*/
    /*public void GetQuestion()
    {
        var questions = QueryMovieById();
        int maxQuestion = 4;
        HashSet<long> Ids = new();
        int index = default;

        var respnse = new List<Question>();

        while (maxQuestion > 0)
        {
            var range = Random.Range(0, questions.Count);
            if (Ids.Contains(questions[range].Id))
            {
                continue;
            }
            Ids.Add(questions[range].Id);

            questions[range].IndexNumber = ++index;// increase index value by 1 each time
            respnse.Add(questions[range]);
            maxQuestion--;
        }
        this.questions = respnse;
    }*/

    /*public void MapQuestionToUI()
    {
        //remove displayed options before loading a new one.
        //RefreshOptions(optionParent);

        //map the corresponding round screen
        StartCoroutine(nameof(MapRoundUI));

        var question = questions.Single(x => x.IndexNumber == currentQuestion);

        questionText.SetText(question.Title);

        int questionOptionIndex = 0;

        questionAnimator.SetTrigger(IS_PLAYING_ANIM_PARAM);

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

            item_go.GetComponentInChildren<TextMeshProUGUI>().text = option.OptionTitle;

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
    }*/

    /*private void RefreshOptions(Transform parent)
    {
        var childCount = parent.childCount;

        questionText.SetText(string.Empty);

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }*/

    /*private void MoveToNexQuestion()
    {
        Debug.Log("MOVE TO THE NEXT QUESTION");

        currentQuestion = Mathf.Clamp(currentQuestion + 1, 1, questions.Count);

        MapQuestionToUI();
    }*/

    /*public void MapRoundUI()
    {
        var totalRound = questions.Count;

        var roundNumber = GameObject.FindGameObjectWithTag("RoundNumber");

        if (roundNumber)
        {
            roundNumber.GetComponent<TextMeshProUGUI>().text = $"ROUND {currentQuestion}";
        }

        var roundDisplay = GameObject.FindGameObjectWithTag("TotalQuestion");

        if (roundDisplay)
        {
            roundDisplay.GetComponent<TextMeshProUGUI>().text = $"{currentQuestion} of {totalRound}";
        }
    }*/

    /*IEnumerator MapRoundUI()
    {
        inPlayMode.SetActive(false);
        roundOverlay.SetActive(true);
        var totalRound = questions.Count;

        var roundNumber = GameObject.FindGameObjectWithTag("RoundNumber");

        if (roundNumber)
        {
            roundNumber.GetComponent<TextMeshProUGUI>().text = $"ROUND {currentQuestion}";
        }

        var roundDisplay = GameObject.FindGameObjectWithTag("TotalQuestion");

        if (roundDisplay)
        {
            roundDisplay.GetComponent<TextMeshProUGUI>().text = $"{currentQuestion} of {totalRound}";
        }
        yield return new WaitForSeconds(3f);
        roundOverlay.SetActive(false);
        inPlayMode.SetActive(true);
    }*/
}
