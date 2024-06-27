using System.Collections;
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

    private void Awake()
    {
        Testing();
    }

    private void Start()
    {
        //StartCoroutine(nameof(OnboardingRoutine));
    }

    IEnumerator OnboardingRoutine()
    {
        ProgressDialogue.Instance.SetLoadingCircleAnimation(true);
        ProgressDialogue.Instance.SetLogoAnimation(true);

        yield return new WaitForSeconds(5f);

        ProgressDialogue.Instance.SetLoadingCircleAnimation(false);
        ProgressDialogue.Instance.SetLogoAnimation(false);

        onboardingPanel.SetActive(false);
        getStartedPanel.SetActive(true);
    }

    IEnumerator ReverseClick(GameObject objectToDeactivate)
    {
        yield return new WaitForSeconds(5f);

        objectToDeactivate.SetActive(false);
        getStartedPanel.SetActive(true);
    }

    private void Testing()
    {
        var getStartedButton = getStartedPanel.transform.GetChild(0).Find("GetStartedButton").GetComponent<Button>();
        var loginButton = getStartedPanel.transform.GetChild(0).Find("LoginButton").GetComponent<Button>();

        if (getStartedButton != null)
        {
            getStartedButton.onClick.RemoveAllListeners();
            getStartedButton.onClick.AddListener(() =>
            {
                getStartedPanel.SetActive(false);
                registerPanel.SetActive(true);

                StartCoroutine(ReverseClick(registerPanel));
            });
        }

        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(() =>
            {
                getStartedPanel.SetActive(false);
                loginPanel.SetActive(true);

                StartCoroutine(ReverseClick(loginPanel));
            });
        }
    }

}
