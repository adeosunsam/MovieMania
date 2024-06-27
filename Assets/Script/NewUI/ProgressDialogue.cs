using System;
using UnityEngine;

public class ProgressDialogue : MonoBehaviour
{
    private int IS_ROTATING_ANIM_PARAM;
    private int IS_LOGO_ANIM_PARAM;

    [SerializeField]
    private Animator loadingCircleAnimator;

    [SerializeField]
    private Animator logoAnimator;

    float countDown = 10f;

    public static ProgressDialogue Instance;

    private void Awake()
    {
        Instance = this;

        IS_ROTATING_ANIM_PARAM = Animator.StringToHash("IsRotating");

        IS_LOGO_ANIM_PARAM = Animator.StringToHash("IsLoading");
    }

    private void Start()
    {
        

        /*SetLoadingCircleAnimation(true);
        SetLogoAnimation(true);*/
    }

    private void Update()
    {
        /*countDown -= Time.deltaTime;
        if (countDown <= 0)
        {
            SetLoadingCircleAnimation(false);
            SetLogoAnimation(false);
        }*/
    }

    /*internal void ResetLoadingPanel(this GameObject loadingPanel, bool value)
    {
        loadingPanel.SetActive(value);
    }*/

    internal void SetLogoAnimation(bool animate)
    {
        logoAnimator.SetBool(IS_LOGO_ANIM_PARAM, animate);
    }

    internal void SetLoadingCircleAnimation(bool animate)
    {
        loadingCircleAnimator.SetBool(IS_ROTATING_ANIM_PARAM, animate);
    }
}
