using System.Collections;
using TMPro;
using UnityEngine;

public class NextRoundOverlay : MonoBehaviour
{
    [SerializeField]
    public GameObject inPlayMode;

    public static NextRoundOverlay Instance;

    internal bool showOverlay;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        showOverlay = true;
    }

    void Update()
    {
        if(showOverlay)
            StartCoroutine(MapRoundRoutine());
    }

    IEnumerator MapRoundRoutine()
    {
        int totalRound = QuestionManager.Singleton.questions.Count;
        int currentQuestion = QuestionManager.Singleton.currentQuestion;

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
        inPlayMode.SetActive(true);

        InPlayMode.Instance.mapQuestion = true;
                
        showOverlay = false;

        gameObject.SetActive(false);
    }
}
