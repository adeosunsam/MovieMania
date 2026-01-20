using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RequestDtos;
using static ResponseDtos.UserActivityResponse;
using static SharedResources;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class ActivityPage : MonoBehaviour
{
    [SerializeField]
    private GameObject activityHeader, userViewBackground, actionView;

    [SerializeField]
    private GameObject pages, inplayPanel;

    [SerializeField]
    private GameObject activityPrefab;

    [SerializeField]
    private Transform m_ContentContainer, actionButton;

    [SerializeField]
    private TextMeshProUGUI challengeHeaderText;

    private Button[] activityHeaderButtons;

    internal bool hasActivityUpdate;

    private ActivityEnum activityEnum;

    private GoogleOAuth googleOAuth;

    public static ActivityPage Singleton;

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
        }
        googleOAuth = FindAnyObjectByType<GoogleOAuth>();

        activityHeaderButtons = activityHeader.GetComponentsInChildren<Button>();
        hasActivityUpdate = true;
    }

    private void OnEnable()
    {
        hasActivityUpdate = true;
        OnclickOuterSpace();
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

            name.text = activity.SenderName;

            activityObject.text = activity.Activity == ActivityEnum.Challenge ? $" challenged you to play {activity.TopicName}"
                : $" wants to be your friend";

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
            else if (button != null && activityEnum == ActivityEnum.Follow)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    SelectedActivity(activity.SenderId);
                });
            }
        }
    }

    private void SelectedActivity(string senderId)
    {
        userViewBackground.SetActive(true);
        actionView.SetActive(true);

        var buttons = actionButton.GetComponentsInChildren<Button>();

        var acceptButton = buttons?.FirstOrDefault(x => x.name == "Accept");
        var declineButton = buttons?.FirstOrDefault(x => x.name == "Decline");

        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                _ = Task.Run(async () =>
                {
                    GameManager.Instance.LoadingPanelInMainThread();
                    var response = await ExternalService.ManageUserAction(new ManageFriendRequest
                    {
                        UserId = UserDetail.UserId,
                        FriendId = senderId,
                        Action = ManageFriend.Accept
                    });

                    if (response.Data)
                    {
						BroadcastService.Singleton.SendActivitiesAsync(senderId);

						googleOAuth.Reload();

                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: true, message: response.ResponseMessage, status: false);
                    }
                    else
                    {
                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: false, message: response.ResponseMessage, status: false);
                    }

                    var getActivity = await ExternalService.GetUserActivity(UserDetail.UserId);
                    UserActivity = getActivity;
                    hasActivityUpdate = true;
                    UserActivity.ForEach(async x =>
                    {
                        x.Sprite = await LoadImageAsync(x.UserImage);
                    });

                    MainThreadDispatcher.Enqueue(() =>
                    {
                        OnclickOuterSpace();
                    });
                });
            });

        }
    }

    //This is called with button onclick
    public void OnclickOuterSpace()
    {
        userViewBackground.SetActive(false);
        actionView.SetActive(false);
    }
}