using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ResponseDtos.UserActivityResponse;
using static SharedResources;

public class ActivityPage : MonoBehaviour
{
    [SerializeField]
    private GameObject activityHeader;

    [SerializeField]
    private GameObject pages, inplayPanel;

    [SerializeField]
    private GameObject activityPrefab;

    [SerializeField]
    private Transform m_ContentContainer;

    [SerializeField]
    private TextMeshProUGUI challengeHeaderText;

    private Button[] activityHeaderButtons;

    internal bool hasActivityUpdate;

    private ActivityEnum activityEnum;

    public static ActivityPage Singleton;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }

        activityHeaderButtons = activityHeader.GetComponentsInChildren<Button>();
        hasActivityUpdate = true;
    }
    void Start()
    {
        if (activityHeaderButtons != null && activityHeaderButtons.Any())
        {
            foreach (var button in activityHeaderButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ActivityHeaderClicked(button));
            }
        }
        activityEnum = ActivityEnum.Follow;

        GetActivity();
    }

    private void Update()
    {
        if (hasActivityUpdate)
        {
            Debug.Log("Updating Challenges Header");
            challengeHeaderText.text = $"Challenges ({(UserActivity != null ? UserActivity.Count(x => x.Activity == ActivityEnum.Challenge) : 0)})";
            GetActivity();
            hasActivityUpdate = false;
        }
    }

    private void ActivityHeaderClicked(Button clickButton)
    {
        ColorUtility.TryParseHtmlString("#CFCFCF", out Color color);

        for (int i = 0; i < activityHeaderButtons.Length; i++)
        {
            var button = activityHeaderButtons[i];

            if (button != clickButton)
            {
                button.GetComponent<Image>().color = color;
                // Enable the button:
                button.interactable = true;
            }
        }

        clickButton.GetComponent<Image>().color = Color.white;

        // Disable the button:
        clickButton.interactable = false;

        if (clickButton.name.StartsWith("Activity"))
        {
            activityEnum = ActivityEnum.Follow;
        }
        else
        {
            activityEnum = ActivityEnum.Challenge;
        }
        hasActivityUpdate = true;
    }

    public void RefreshAvailableLevels(int childCount, Transform parent)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void GetActivity()
    {
        int childCount = m_ContentContainer.childCount;

        RefreshAvailableLevels(childCount, m_ContentContainer);

        if (UserActivity == null) return;

        foreach (var activity in UserActivity.Where(x => x.Activity == activityEnum))
        {
            var item_go = Instantiate(activityPrefab);
            item_go.transform.SetParent(m_ContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var image = item_go.GetComponentInChildren<Image>().transform;

            var profileImage = image.Find("ImageMask").GetComponent<Image>();

            profileImage.sprite = activity.Sprite;

            var textGameObject = item_go.GetComponentsInChildren<Transform>().First(c => !c.Find("Line"));

            var texts = textGameObject.GetComponentsInChildren<TextMeshProUGUI>();

            var name = texts.First(x => x.name == "Name");
            var activityObject = texts.First(x => x.name == "Activity");

            name.text = activity.ChallengerName;

            activityObject.text = activity.Activity == ActivityEnum.Challenge ? $" challenged you to play {activity.TopicName}"
                : $" started following you";

            var button = item_go.GetComponentInChildren<Button>();

            if (button != null && activityEnum == ActivityEnum.Challenge)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    inplayPanel.SetActive(true);
                    pages.SetActive(false);
                    BroadcastService.Singleton.JoinAndPlay(activity.Id, activity.TopicId);
                });
            }
        }
    }
}