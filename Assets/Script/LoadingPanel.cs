using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private Animator loadingCircleAnimator;

    void OnEnable()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    private void OnDisable()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
    }
}
