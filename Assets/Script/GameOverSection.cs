using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSection : MonoBehaviour
{

    [SerializeField]
    private GameObject loadingView;
    
    [SerializeField]
    private Animator loadingCircleAnimator;

    private bool gameEnded;

    public static GameOverSection Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;

        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    private void Update()
    {
        if (gameEnded)
        {
            DisplayScoreCard();
            gameEnded = false;
        }
    }

    private void DisplayScoreCard()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
        loadingView.SetActive(false);
    }

}
