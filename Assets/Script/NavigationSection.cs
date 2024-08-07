using System;
using UnityEngine;
using UnityEngine.UI;

public class NavigationSection : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedTopic;

    [SerializeField]
    private GameObject backButton;

    public event EventHandler<TopicResponseDto> OnTopicSelected;

    public static NavigationSection Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public virtual void OnClickTopicCard(GameObject topicPrefab, GameObject currentPage, TopicResponseDto topic)
    {
        var button = topicPrefab.GetComponentInChildren<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                currentPage.SetActive(false);
                selectedTopic.SetActive(true);
                backButton.SetActive(true);

                OnTopicSelected?.Invoke(this, topic);

                OnClickBackButton(currentPage);
            });
        }
    }

    private void OnClickBackButton(GameObject currentPage)
    {
        var button = backButton.GetComponentInChildren<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                currentPage.SetActive(true);
                backButton.SetActive(false);
                selectedTopic.SetActive(false);
            });
        }
    }
}
