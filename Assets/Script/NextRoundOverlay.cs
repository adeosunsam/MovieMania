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
    }
}
