using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestionDto;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questionTitle;

    [SerializeField]
    private TextMeshProUGUI Score2;

    [SerializeField]
    private Transform optionParent;

    [SerializeField]
    private GameObject optionPrefab;

    public static QuestionManager Singleton;

    private List<QuestionDto.Question> questions;

    private int currentQuestion = 1;
    private void Awake()
    {
        Singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        MainUI.Singleton.OnTimerCountdown += Question_OnTimerCountdown;

        GetQuestion();
    }

    private void Question_OnTimerCountdown(object sender, System.EventArgs e)
    {
        //Move to next question
        MoveToNexQuestion();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<QuestionDto.Question> QueryMovieById()
    {
        return new List<QuestionDto.Question>
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
                        OptionAlphabet = "A",
                        OptionTitle = "Ayodeji",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "B",
                        OptionTitle = "Toluwalope",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "C",
                        OptionTitle = "Samuel",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "D",
                        OptionTitle = "Tayo",
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
                        OptionAlphabet = "A",
                        OptionTitle = "Ibadan",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "B",
                        OptionTitle = "Abidjan",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "C",
                        OptionTitle = "Lagos",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "D",
                        OptionTitle = "Kumasi",
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
                        OptionAlphabet = "A",
                        OptionTitle = "Burna Boy",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "B",
                        OptionTitle = "Baddo",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "C",
                        OptionTitle = "Wizzy",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "D",
                        OptionTitle = "O.B.O Baddest",
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
                        OptionAlphabet = "A",
                        OptionTitle = "Burna Boy",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "B",
                        OptionTitle = "Baddo",
                        IsCorrectOption = false
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "C",
                        OptionTitle = "Wizzy",
                        IsCorrectOption = true
                    },
                    new QuestionDto.QuestionOption
                    {
                        OptionAlphabet = "D",
                        OptionTitle = "O.B.O Baddest",
                        IsCorrectOption = false
                    }
                }
            },

        };
    }

    public void GetQuestion()
    {
        var questions = QueryMovieById();
        int maxQuestion = 3;
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

            questions[range].IndexNumber = ++index;
            respnse.Add(questions[range]);
            maxQuestion--;
        }

        this.questions = respnse;
    }

    public void MapQuestionToUI()
    {
        //remove displayed options before loading a new one.
        RefreshOptions(optionParent);

        var question = questions.Single(x => x.IndexNumber == currentQuestion);

        questionTitle.SetText(question.Title);


        foreach (var option in question.Options)
        {
            var item_go = Instantiate(optionPrefab);
            item_go.transform.SetParent(optionParent);
            //reset the item's scale - this can get munged with UI prefabs
            //item_go.transform.localScale = Vector2.one;

            item_go.transform.Find("OptionAlphabet").GetComponent<TextMeshProUGUI>().text = $"{option.OptionAlphabet}.";
            item_go.transform.Find("OptionTitle").GetComponent<TextMeshProUGUI>().text = $"{option.OptionTitle}.";

            // Add an onClick listener to the button component
            Button button = item_go.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    MainUI.Singleton.PlayerScore();
                    MoveToNexQuestion();
                });
            }
        }
    }

    private void RefreshOptions(Transform parent)
    {
        var childCount = parent.childCount;

        questionTitle.SetText(string.Empty);

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private void MoveToNexQuestion()
    {
        Debug.Log("MOVE TO THE NEXT QUESTION");
        currentQuestion = Mathf.Clamp(currentQuestion + 1, 1, questions.Count);

        MapQuestionToUI();
    }
}
