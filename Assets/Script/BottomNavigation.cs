using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomNavigation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI headerText;

    [SerializeField]
    private GameObject homePage;

    private GameObject previousActiveMenuPage;

    public GameObject[] menuButtons;

    private void Awake()
    {
        previousActiveMenuPage = previousActiveMenuPage != null ? previousActiveMenuPage : homePage;
    }

    public void OnMenuClick(GameObject gameObject)
    {
        ColorUtility.TryParseHtmlString("#290916", out Color clicked);
        ColorUtility.TryParseHtmlString("#A6A6A6", out Color unClicked);

        DeactivateBackButton();

        foreach (var menuItem in menuButtons)
        {
            var button = menuItem.GetComponent<Button>();
            var image = menuItem.GetComponentInChildren<Image>();
            var menuName = menuItem.GetComponentInChildren<TextMeshProUGUI>();

            if (gameObject == menuItem)
            {
                headerText.text = menuItem.name;

                if (button != null)
                {
                    button.interactable = false;
                }


                if (!menuItem.name.StartsWith("Play"))
                {
                    if (image != null)
                        image.color = clicked;
                    if (menuName != null)
                        menuName.color = clicked;
                }

                continue;
            }
            if (button != null)
                button.interactable = true;

            if (!menuItem.name.StartsWith("Play"))
            {
                if (image != null)
                    image.color = unClicked;
                if (menuName != null)
                    menuName.color = unClicked;
            }
        }
    }

    private void DeactivateBackButton()
    {
        var navSection = FindAnyObjectByType<NavigationSection>();

        if (navSection == null)
            return;

        navSection.DeactivateBackButtonOnMenuClick();

        /*button.onClick.RemoveAllListeners();
        button.onClick.AddListener(navSection.DeactivateBackButtonOnMenuClick);

        button.onClick.Invoke();*/
    }

    public void OnClickPage(GameObject page)
    {
        if (previousActiveMenuPage != null)
            previousActiveMenuPage.SetActive(false);

        page.SetActive(true);

        var activity = FindAnyObjectByType<ActivityPage>();
        if (activity != null && activity.gameObject.activeInHierarchy)
        {
            Debug.Log($"HAS ACTIVITY PAGE: {activity}");
            activity.hasActivityUpdate = true;
        }

        previousActiveMenuPage = page;
    }

    /*public void OnclickActivity()
    {
        headerText.text = "Activity";

        profile.SetActive(false);
        startgame.SetActive(false);
        topic.SetActive(false);
        activityChallenge.SetActive(true);
    }
    public void OnclickTopics()
    {
        headerText.text = "Topics";

        profile.SetActive(false);
        startgame.SetActive(false);
        topic.SetActive(true);
        activityChallenge.SetActive(false);
    }
    public void OnclickHome()
    {
        headerText.text = "Home";

        profile.SetActive(false);
        startgame.SetActive(false);
        topic.SetActive(false);
        activityChallenge.SetActive(false);
    }

    public void OnclickBackbutton()
    {
        topicPage.SetActive(false);
        startgame.SetActive(true);
        activityChallenge.SetActive(false);
    }

    public void OnclickTopicButton()
    {
        topicPage.SetActive(true);
        startgame.SetActive(false);
        activityChallenge.SetActive(false);

    }

    public void OnClickGameLogo()
    {
        headerText.text = string.Empty;

        profile.SetActive(false);
        startgame.SetActive(true);
        topic.SetActive(false);
        activityChallenge.SetActive(false);
    }

    public void OnclickProfile()
    {
        headerText.text = "Profile";

        profile.SetActive(true);
        startgame.SetActive(false);
        topic.SetActive(false);
        activityChallenge.SetActive(false);
    }*/
}
