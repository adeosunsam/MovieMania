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
    private GameObject pages,inplayPanel;

    void Awake()
    {
        NavigationSection.Instance.OnTopicSelected += Instance_OnTopicSelected;
    }

    private void Instance_OnTopicSelected(object sender, TopicResponseDto e)
    {
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
                inplayPanel.SetActive(true);
                pages.SetActive(false);
                BroadcastService.Singleton.Authenticate();
            });
        }
    }


}
