using UnityEngine;

public class GameOverSection : MonoBehaviour
{

    [SerializeField]
    private GameObject loadingView, scoreCardView;

    [SerializeField]
    private Animator loadingCircleAnimator;

    private GoogleOAuth googleOAuth;

    private void Start()
    {
        googleOAuth = FindAnyObjectByType<GoogleOAuth>();
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
            googleOAuth.Reload();
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
