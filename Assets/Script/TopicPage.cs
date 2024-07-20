using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class TopicPage : MonoBehaviour
{
    [SerializeField]
    private Transform sectionContentContainer;

    [SerializeField]
    private GameObject sectionPrefab;

    [SerializeField]
    private GameObject topicPrefab;

    private Transform topicContentContainer;

    [SerializeField]
    private Animator loadingCircleAnimator;

    private NavigationSection onClickTopic;
    private bool hasDisplayedTopics;

    void Start()
    {
        onClickTopic = FindAnyObjectByType<NavigationSection>();
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }
    private void Update()
    {
        if (TopicResponse != null && !TopicResponse.Exists(x => !x?.Sprite) && !hasDisplayedTopics)
        {
            hasDisplayedTopics = true;
            ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);
            loadingCircleAnimator.gameObject.SetActive(false);
            GetTopics();
        }
    }

    public void GetTopics()
    {
        var groupedTopics = TopicResponse.GroupBy(x => x.Category).ToList();

        foreach (var groupedTopic in groupedTopics)
        {
            var item_go = Instantiate(sectionPrefab);

            item_go.transform.SetParent(sectionContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var topicTitle = item_go.GetComponentInChildren<TextMeshProUGUI>();

            topicTitle.text = groupedTopic.Key.ToString();

            var nestedScrollRect = item_go.GetComponentInChildren<ScrollRect>();

            topicContentContainer = nestedScrollRect.content;

            foreach (var topic in groupedTopic)
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
                    onClickTopic.OnClickTopicCard(topic_go, gameObject, topic);
                }
            }
        }
    }
}