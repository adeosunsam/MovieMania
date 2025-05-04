using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AppConstant;
using static SharedResources;

public class StartPage : MonoBehaviour
{
    [SerializeField]
    private Transform sectionContentContainer, friendContentContainer;

    [SerializeField]
    private GameObject sectionPrefab, topicPrefab, friendPrefab;

    private Transform topicContentContainer;

    [SerializeField]
    private Animator startPageLoadingCircleAnimator, friendLoadingAnimator;

    private NavigationSection onClickTopic;

    private bool hasDisplayedFollowedTopics;
    private bool hasDisplayedUserFriends;

    void Start()
    {
        onClickTopic = FindAnyObjectByType<NavigationSection>();

        /*_ = Task.Run(async () =>
        {
            TopicResponse = await ExternalService.FetchAvailableTopics(UserDetail.UserId);
            Debug.LogWarning($"TopicResponse COunt:{TopicResponse?.Count}");
            Debug.LogWarning($"TopicResponse: {TopicResponse != null}");
            SharedResources.StartUp();
        });*/
    }

    private void OnEnable()
    {
        Debug.Log("START PAGE ENABLED");
        startPageLoadingCircleAnimator.gameObject.SetActive(true);
        hasDisplayedFollowedTopics = false;

        ProgressDialogue.Instance.SetLoadingCircleAnimation(startPageLoadingCircleAnimator, true);
        ProgressDialogue.Instance.SetLoadingCircleAnimation(friendLoadingAnimator, true);
    }

    private void Update()
    {
        if (TopicResponse != null && !TopicResponse.Exists(x => !x?.Sprite) && !hasDisplayedFollowedTopics)
        {
            hasDisplayedFollowedTopics = true;

            ProgressDialogue.Instance.SetLoadingCircleAnimation(startPageLoadingCircleAnimator, false);

            startPageLoadingCircleAnimator.gameObject.SetActive(false);
            FollowedTopic();
            //keep loading dialogue while hasDisplayedFollowedTopics = false
        } 
    }

    private void LateUpdate()
    {
        if (UserFriends != null && (!UserFriends.Any() || UserFriends.Exists(x => x.Sprite)) && !hasDisplayedUserFriends)
        {
            hasDisplayedUserFriends = true;

            ProgressDialogue.Instance.SetLoadingCircleAnimation(friendLoadingAnimator, false);
            friendLoadingAnimator.gameObject.SetActive(false);
            MapFriends();
        }
    }

    private void FollowedTopic()
    {
        var followedTopics = TopicResponse.Where(x => x.IsFollowed);

        int childCount = sectionContentContainer.childCount;

        RefreshContentSection(childCount, sectionContentContainer);

        var item_go = Instantiate(sectionPrefab);

        item_go.transform.SetParent(sectionContentContainer);

        //reset the item's scale -- this can get munged with UI prefabs
        item_go.transform.localScale = Vector2.one;

        var topicTitle = item_go.GetComponentInChildren<TextMeshProUGUI>();

        topicTitle.text = FollowedTopicHeader;

        var nestedScrollRect = item_go.GetComponentInChildren<ScrollRect>();

        topicContentContainer = nestedScrollRect.content;

        foreach (var topic in followedTopics)
        {
            var topic_go = Instantiate(topicPrefab);

            topic_go.transform.SetParent(topicContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            topic_go.transform.localScale = Vector2.one;

            var profileImage = topic_go.GetComponentInChildren<Image>();

            profileImage.sprite = topic.Sprite;
            //_ = LoadTopicImageAsync(profileImage, topic.Image);

            topic_go.GetComponentInChildren<TextMeshProUGUI>().text = topic.Name;

            if (onClickTopic)
            {
                var button = topic_go.GetComponentInChildren<Button>();

                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        onClickTopic.OnClickTopicCard(gameObject, topic);
                    });
                }
            }
        }
    }

    public void MapFriends()
    {
        int childCount = friendContentContainer.childCount;

        RefreshContentSection(childCount, friendContentContainer, 1);

        //TODO: work on loading page and for now, display all user on the app except current player
        if (UserFriends == null)
        {
            return;
        }

        foreach (var user in UserFriends)
        {
            var item_go = Instantiate(friendPrefab);

            item_go.transform.SetParent(friendContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var userName = item_go.GetComponentInChildren<TextMeshProUGUI>();

            userName.text = $"{user.FirstName} {user.LastName}";

            var parentImageTransform = item_go.GetComponentInChildren<Image>().transform;

            var profileImage = parentImageTransform.Find("ImageMask").GetComponent<Image>();

            Debug.LogWarning($"User Image: {user.Sprite != null} for {userName.text}");
            profileImage.sprite = user.Sprite != null ? user.Sprite : profileImage.sprite;

            var button = item_go.GetComponentInChildren<Button>();

            //if (button != null)
            //{
            //    button.onClick.RemoveAllListeners();
            //    button.onClick.AddListener(() =>
            //    {
            //        inplayPanel.SetActive(true);
            //        pages.SetActive(false);
            //        BroadcastService.Singleton.PlayGame(topicId);
            //    });
            //}
        }
    }

    public void RefreshContentSection(int childCount, Transform parent, int excludeFrom = 0)
    {
        for (int i = excludeFrom; i < childCount; i++) //exclude the first count
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}
