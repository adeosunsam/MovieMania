using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class StartPage : MonoBehaviour
{
    private readonly List<FollowedTopic> topics = new()
    {
        new FollowedTopic("0b1b0150-54f8-42f0-92e5-8795986e9939"),
        new FollowedTopic("07e2d314-529f-42d1-8d0b-46b0ed8d8ee1"),
        new FollowedTopic("3998b12b-e36b-4d59-b2a4-efc6a58d0fc3"),
        new FollowedTopic("2c1a2a77-7d35-4b68-8c4a-5a1f1ae9bc12"),
        new FollowedTopic("be39a7d7-fbb6-42e7-8102-59e4cc282748"),
    };

    [SerializeField]
    private Transform sectionContentContainer;

    [SerializeField]
    private GameObject sectionPrefab;

    [SerializeField]
    private GameObject topicPrefab;

    private Transform topicContentContainer;

    private NavigationSection onClickTopic;

    void Start()
    {
        onClickTopic = FindAnyObjectByType<NavigationSection>();
        FollowedTopic();
    }

    private void FollowedTopic()
    {
        var followedTopics = SharedResources.topics.Where(x => topics.Select(y => y.TopicId).Contains(x.Id));

        var item_go = Instantiate(sectionPrefab);

        item_go.transform.SetParent(sectionContentContainer);

        //reset the item's scale -- this can get munged with UI prefabs
        item_go.transform.localScale = Vector2.one;

        var topicTitle = item_go.GetComponentInChildren<TextMeshProUGUI>();

        topicTitle.text = "Followed Topics";

        var nestedScrollRect = item_go.GetComponentInChildren<ScrollRect>();

        topicContentContainer = nestedScrollRect.content;

        foreach (var topic in followedTopics)
        {
            var topic_go = Instantiate(topicPrefab);

            topic_go.transform.SetParent(topicContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            topic_go.transform.localScale = Vector2.one;

            var profileImage = topic_go.GetComponentInChildren<Image>();

            Texture2D texture = LoadTextureFromBase64(topic.Image);

            // Create a sprite from the texture
            if (texture != null)
            {
                Sprite sprite = SpriteFromTexture2D(texture);

                profileImage.sprite = sprite;
            }

            topic_go.GetComponentInChildren<TextMeshProUGUI>().text = topic.Name;

            if(onClickTopic)
            {
                onClickTopic.OnClickTopicCard(topic_go, gameObject, topic);
            }
        }

    }
}
