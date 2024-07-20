using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;
using static AppConstant;

public class StartPage : MonoBehaviour
{
    [SerializeField]
    private Transform sectionContentContainer;

    [SerializeField]
    private GameObject sectionPrefab;

    [SerializeField]
    private GameObject topicPrefab;

    private Transform topicContentContainer;

    private NavigationSection onClickTopic;

    private bool hasDisplayedFollowedTopics;

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

    private void Update()
    {
        if(TopicResponse != null && !TopicResponse.Exists(x => !x?.Sprite) && !hasDisplayedFollowedTopics)
        {
            hasDisplayedFollowedTopics = true;
            FollowedTopic();
            //keep loading dialogue while hasDisplayedFollowedTopics = false
        }
    }

    private void FollowedTopic()
    {
        var followedTopics = TopicResponse.Where(x => x.IsFollowed);

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

            if(onClickTopic)
            {
                onClickTopic.OnClickTopicCard(topic_go, gameObject, topic);
            }
        }

    }
}
