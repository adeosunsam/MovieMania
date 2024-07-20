using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class SelectedTopic : MonoBehaviour
{
    private GameObject selectedContent;

    void Awake()
    {
        NavigationSection.Instance.OnTopicSelected += Instance_OnTopicSelected;
    }

    private void Instance_OnTopicSelected(object sender, TopicResponseDto e)
    {
        Test(e);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private async void Test(TopicResponseDto topic)
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
                    text.text = topic.QuestionCount.ToString();
                    break;
                case "Followers":
                    text.text = topic.FollowersCount.ToString();
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
    }
}
