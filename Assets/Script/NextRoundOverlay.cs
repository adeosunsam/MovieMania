using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextRoundOverlay : MonoBehaviour
{
    [SerializeField]
    private GameObject inPlayMode;

    [SerializeField]
    private TextMeshProUGUI roundNumberTextMesh, totalQuestionTextMesh, title, description;

    [SerializeField]
    private Image titleImageHolder;

    public static NextRoundOverlay Instance;

    internal bool showOverlay;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        MapNextRoundData();
        showOverlay = true;
    }

    void FixedUpdate()
    {
        if (showOverlay)
        {
            StartCoroutine(MapRoundRoutine());
        }
    }

    //This might not be needed on the long run
    private void RefreshTexts()
    {
        roundNumberTextMesh.text = "";
        totalQuestionTextMesh.text = "";
        title.text = "";
        description.text = "";
        titleImageHolder.sprite = null;
    }

    IEnumerator MapRoundRoutine()
    {
        yield return new WaitForSeconds(3f);
        inPlayMode.SetActive(true);

        InPlayMode.Instance.mapQuestion = true;
                
        showOverlay = false;

        RefreshTexts();

        gameObject.SetActive(false);
    }

    internal void MapNextRoundData()
    {
        int totalRound = QuestionManager.Singleton.questions.Count;
        int currentQuestion = QuestionManager.Singleton.currentQuestion;

        roundNumberTextMesh.text = $"ROUND {currentQuestion}";
        totalQuestionTextMesh.text = $"{currentQuestion} of {totalRound}";

        var inplayTopic = SharedResources.TopicInPlay;

        //This should not happen at any point in time.
        if (inplayTopic == null)
            return; //Log an error

        title.text = inplayTopic.Name;
        description.text = inplayTopic.Description;
        titleImageHolder.sprite = inplayTopic.Sprite;

        /*var roundNumber = GameObject.FindGameObjectWithTag("RoundNumber");

        if (roundNumber)
        {
            roundNumber.GetComponent<TextMeshProUGUI>().text = $"ROUND {currentQuestion}";
        }

        var roundDisplay = GameObject.FindGameObjectWithTag("TotalQuestion");

        if (roundDisplay)
        {
            roundDisplay.GetComponent<TextMeshProUGUI>().text = $"{currentQuestion} of {totalRound}";
        }*/



        /*var texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            switch (text.name)
            {
                case "Title":
                    text.text = inplayTopic.Name;
                    break;
                case "Description":
                    text.text = inplayTopic.Description;
                    break;
                default:
                    break;
            }
        }
*/

        /*var image = gameObject.GetComponentInChildren<Image>();

        if (image != null)
            image.sprite = inplayTopic.Sprite;*/
    }
}
