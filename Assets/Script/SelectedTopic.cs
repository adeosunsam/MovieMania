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

    private void Instance_OnTopicSelected(object sender, Topic e)
    {
        Test(e);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Test(Topic topic)
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
                default:
                    break;
            }
        }
    }
}
