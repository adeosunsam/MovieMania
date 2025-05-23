using System;
using UnityEngine;
using UnityEngine.UI;
using static RequestDtos;
using static ResponseDtos;

public class NavigationSection : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedTopicPage, inviteFriendPage, homePage;

    [SerializeField]
    private GameObject backButton;

    private GameObject currentPage;

    public event EventHandler<TopicResponseDto> OnTopicSelected;

    public event EventHandler<UserDetailDto> OnUserSelected;

    public static NavigationSection Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        currentPage = homePage;
    }

    public virtual void OnClickTopicCard(GameObject currentPage, TopicResponseDto topic)
    {
        this.currentPage = currentPage;

        currentPage.SetActive(false);
        selectedTopicPage.SetActive(true);
        backButton.SetActive(true);

        OnTopicSelected?.Invoke(this, topic);

        OnClickBackButton(currentPage);
    }

    public void OnclickInviteFriend()
    {
        homePage.SetActive(false);
        inviteFriendPage.SetActive(true);

        currentPage = inviteFriendPage;
    }

    public void OnclickFriendInList(GameObject friendObject, UserDetailDto user)
    {
        currentPage.SetActive(false);
        friendObject.SetActive(true);

        currentPage = friendObject;

        OnUserSelected?.Invoke(this, user);
    }

    internal void OnClickBackButton(GameObject currentPage)
    {
        var button = backButton.GetComponentInChildren<Button>();

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                currentPage.SetActive(true);
                backButton.SetActive(false);
                selectedTopicPage.SetActive(false);
            });
        }
    }

    internal void DeactivateBackButtonOnMenuClick()
    {
        if (!currentPage)
            return;

        var button = backButton.GetComponentInChildren<Button>();

        if (button != null)
            button.onClick.Invoke();

        currentPage.SetActive(false);
        currentPage = null;
    }
}
