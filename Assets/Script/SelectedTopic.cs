using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ResponseDtos;

public class SelectedTopic : MonoBehaviour
{
    private GameObject selectedContent;

    [SerializeField]
    private Button play;

    [SerializeField]
    private GameObject pages,inplayPanel, userViewBackground, userView;

    [SerializeField]
    private GameObject sectionPrefab;

    [SerializeField]
    private Transform sectionContentContainer;

    void Awake()
    {
        NavigationSection.Instance.OnTopicSelected += Instance_OnTopicSelected;
    }

    private void Instance_OnTopicSelected(object sender, TopicResponseDto e)
    {
        OnclickOuterSpace();
        TopicDetails(e);
    }

    private void OnApplicationQuit()
    {
        NavigationSection.Instance.OnTopicSelected -= Instance_OnTopicSelected;
    }

    private void TopicDetails(TopicResponseDto topic)
    {
        var nestedScrollRect = GetComponentInChildren<ScrollRect>();

        selectedContent = nestedScrollRect.content.gameObject;

        var texts = selectedContent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            switch (text.name)
            {
                case "Title":
                    text.text = topic.Name;
                    break;
                case "Description":
                    text.text = topic.Description;
                    break;
                case "Games":
                    text.text = $"{topic.QuestionCount}";
                    break;
                case "Followers":
                    text.text = $"{topic.FollowersCount}";
                    break;
                case "Friends":
                    text.text = "20";//topic.FriendsCount.ToString();
                    break;
                default:
                    break;
            }
        }

        var images = selectedContent.GetComponentsInChildren<Image>();

        var image = images.FirstOrDefault(x => x.name == "Selected Topic Image");

        if (image != null)
            image.sprite = topic.Sprite;

        if(play != null)
        {
            play.onClick.RemoveAllListeners();
            play.onClick.AddListener(() =>
            {
                SelectedUserToPlay(topic.Id);
            });
        }
    }

    public void MapFriends(string topicId)
    {
        int childCount = sectionContentContainer.childCount;

        RefreshAvailableFriends(childCount, sectionContentContainer);

        //TODO: work on loading page and for now, display all user on the app except current player
        if(SharedResources.UserFriends == null)
        {
            return;
        }

        foreach (var user in SharedResources.UserFriends)
        {
            var item_go = Instantiate(sectionPrefab);

            item_go.transform.SetParent(sectionContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var userName = item_go.GetComponentInChildren<TextMeshProUGUI>();

            userName.text = $"{user.FirstName} {user.LastName}";

            //var profileImage = item_go.GetComponentInChildren<Image>();

            //profileImage.sprite = u.Sprite;

            var button = item_go.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    inplayPanel.SetActive(true);
                    pages.SetActive(false);
                    BroadcastService.Singleton.Authenticate(topicId);
                });
            }
        }
    }

    public void RefreshAvailableFriends(int childCount, Transform parent)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private void SelectedUserToPlay(string topicId)
    {
        userViewBackground.SetActive(true);
        userView.SetActive(true);

        MapFriends(topicId);
        /*inplayPanel.SetActive(true);
        pages.SetActive(false);
        BroadcastService.Singleton.Authenticate(topicId);*/
    }

    public void OnclickOuterSpace()
    {
        userViewBackground.SetActive(false);
        userView.SetActive(false);
    }
}
