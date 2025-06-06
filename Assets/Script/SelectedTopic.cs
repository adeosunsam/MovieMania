using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using static ResponseDtos;
using static SharedResources;

public class SelectedTopic : MonoBehaviour
{
    private GameObject selectedContent;

    [SerializeField]
    private Button play, followButton;

    [SerializeField]
    private GameObject pages, inplayPanel, userViewBackground, userView;

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

        if (topic.IsFollowed)
        {
            followButton.interactable = false;
        }
        else
        {
            followButton.interactable = true;
        }

        if (play != null)
        {
            play.onClick.RemoveAllListeners();
            play.onClick.AddListener(() =>
            {
                SelectedUserToPlay(topic.Id);
            });
        }

        if (followButton != null)
        {
            followButton.onClick.RemoveAllListeners();
            followButton.onClick.AddListener(() =>
            {
                _ = Task.Run(async () =>
                {
                    GameManager.Instance.LoadingPanelInMainThread();
                    var isSuccessful = await ExternalService.FollowTopic(UserDetail.UserId, topic.Id);
                    
                    if (isSuccessful)
                    {
                        var incomingTopics = await ExternalService.FetchAvailableTopics(UserDetail.UserId);
                        MainThreadDispatcher.Enqueue(() =>
                        {
                            PlayerPrefExtension<List<TopicResponseDto>>.UpdateDb(incomingTopics);
                        });
                        TopicResponse = incomingTopics;
                        StartUp();
                        
                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: true, message: "topic followed successfully", status: false);
                    }
                    else
                    {
                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: false, message: "Unable to follow topic", status: false);
                    }
                });
            });
        }
    }

    //private void LoadingPanelInMainThread(bool isSuccessful = true, string message = "success", bool status = true)
    //{
    //    MainThreadDispatcher.Enqueue(() =>
    //    {
    //        try
    //        {
    //            //StartCoroutine(LoadPanelRoutine());
    //            if (loadingPanel == null)
    //            {
    //                return;
    //            }

    //            if(!status)
    //                ToastNotification.Show(message, isSuccessful ? "success" : "error");

    //            loadingPanel.gameObject.SetActive(status);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError(ex.Message);
    //        }
    //    });
    //}

    public void MapFriends(string topicId)
    {
        int childCount = sectionContentContainer.childCount;

        RefreshAvailableFriends(childCount, sectionContentContainer);

        //TODO: work on loading page and for now, display all user on the app except current player
        if (UserFriends == null)
        {
            return;
        }

        foreach (var user in UserFriends)
        {
            var item_go = Instantiate(sectionPrefab);

            item_go.transform.SetParent(sectionContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var userName = item_go.GetComponentInChildren<TextMeshProUGUI>();

            userName.text = $"{user.FirstName} {user.LastName}";

            var profileImage = item_go.GetComponentInChildren<Image>();

            profileImage.sprite = user.Sprite != null ? user.Sprite : profileImage.sprite;

            var button = item_go.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    inplayPanel.SetActive(true);
                    pages.SetActive(false);
                    OpponentDetail = user;
                    BroadcastService.Singleton.PlayGame(topicId);
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
