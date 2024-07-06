using TMPro;
using UnityEngine;

public class BottomNavigation : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI headerText;

    public GameObject profile;
    public GameObject startgame;
    public GameObject topic;
    public GameObject backButton;
    public GameObject topicPage;
    public GameObject activityChallenge;

    void Awake()
    {
        profile.SetActive(false);
        startgame.SetActive(false);
        topic.SetActive(false);
        activityChallenge.SetActive(false);
    }


    public void OnclickActivity()
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
        backButton.SetActive(false);
        topicPage.SetActive(false);
        startgame.SetActive(true);
        activityChallenge.SetActive(false);
    }

    public void OnclickTopicButton()
    {
        backButton.SetActive(true);
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
    }
}
