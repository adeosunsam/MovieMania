using UnityEngine;

public class ProgressDialogue : MonoBehaviour
{
    private int IS_ROTATING_ANIM_PARAM;
    private int IS_LOGO_ANIM_PARAM;

    public static ProgressDialogue Instance;

    private void Awake()
    {
        Instance = this;

        IS_ROTATING_ANIM_PARAM = Animator.StringToHash("IsRotating");

        IS_LOGO_ANIM_PARAM = Animator.StringToHash("IsLoading");
    }

    internal void SetLogoAnimation(Animator logoAnimator, bool animate)
    {
        logoAnimator.SetBool(IS_LOGO_ANIM_PARAM, animate);
    }

    internal void SetLoadingCircleAnimation(Animator circleAnimator, bool animate)
    {
        if (!circleAnimator.gameObject.activeInHierarchy)
        {
            return;
        }
        circleAnimator.SetBool(IS_ROTATING_ANIM_PARAM, animate);
    }
}
