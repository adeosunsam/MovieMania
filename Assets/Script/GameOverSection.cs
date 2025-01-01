using UnityEngine;

public class GameOverSection : MonoBehaviour
{

    [SerializeField]
    private GameObject loadingView, scoreCardView;

    [SerializeField]
    private Animator loadingCircleAnimator;

    //internal bool gameEnded;

    //public static GameOverSection Singleton { get; private set; }

    private void Awake()
    {
        //if (Singleton == null)
        //    Singleton = this;

        
    }

    private void OnEnable()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    private void Update()
    {
        if (BroadcastService.Singleton.OpponentGameOver)
        {
            DisplayScoreCard();
            BroadcastService.Singleton.OpponentGameOver = false;
        }
    }

    private void DisplayScoreCard()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
        loadingView.SetActive(false);
        scoreCardView.SetActive(true);
    }
}
