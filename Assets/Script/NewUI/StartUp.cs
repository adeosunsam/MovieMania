using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour
{
    [SerializeField]
    private GameObject onboardingPanel;

    [SerializeField]
    private GameObject getStartedPanel;


    [SerializeField]
    private GameObject loginPanel;


    [SerializeField]
    private GameObject registerPanel;

    [SerializeField]
    private Animator loadingCircleAnimator, logoAnimator;

    private void Start()
    {
        Testing();
        StartCoroutine(nameof(OnboardingRoutine));
    }

    IEnumerator OnboardingRoutine()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
        ProgressDialogue.Instance.SetLogoAnimation(logoAnimator, true);

        yield return new WaitForSeconds(5f);

        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
        ProgressDialogue.Instance.SetLogoAnimation(logoAnimator, false);

        onboardingPanel.SetActive(false);
        getStartedPanel.SetActive(true);
    }
/*
    IEnumerator ReverseClick(GameObject objectToDeactivate)
    {
        yield return new WaitForSeconds(5f);

        objectToDeactivate.SetActive(false);
        getStartedPanel.SetActive(true);
    }
*/
    private void Testing()
    {
        var buttons = getStartedPanel.GetComponentsInChildren<Button>();
        var getStartedButton = buttons.FirstOrDefault(x => x.name == "GetStartedButton");

        if (getStartedButton != null)
        {
            getStartedButton.onClick.RemoveAllListeners();
            getStartedButton.onClick.AddListener(() =>
            {
                getStartedPanel.SetActive(false);
                registerPanel.SetActive(true);

                // Make sure MainThreadDispatcher is in the scene
                if (FindObjectOfType<MainThreadDispatcher>() == null)
                {
                    GameObject dispatcherObj = new(nameof(MainThreadDispatcher));
                    dispatcherObj.AddComponent<MainThreadDispatcher>();
                }
                //StartCoroutine(ReverseClick(registerPanel));
            });
        }
    }

}
