using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RequestDtos;
using static SharedResources;

public class UserProfile : MonoBehaviour
{
    [SerializeField]
    private GameObject profilePics, loadingPanel;

    [SerializeField]
    private TextMeshProUGUI fullName, state, totalGamePlayed, followers, friends;

    private bool hasDisplayProfilePics;
    private bool hasDisplayUserStat;

    [SerializeField]
    private Animator loadingCircleAnimator;

    void Start()
    {
        LoadNameAndState(UserDetail);
        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, true);
    }

    void Update()
    {
        if (!hasDisplayProfilePics && (UserDetail?.Sprite != null || (bool)UserDetail?.IsImageLoadingStopped))
        {
            ProfilePage();
            hasDisplayProfilePics = true;
        }
        if(!hasDisplayUserStat && GamingCount != null)
        {
            LoadUserStatistics();
            hasDisplayUserStat = true;
        }
    }

    public void LoadUserStatistics()
    {
        totalGamePlayed.text = $"{GamingCount.TotalGamePlayed}";
        friends.text = $"{GamingCount.TotalFriends}";
    }

    private void LoadNameAndState(UserDetailDto user)
    {
        fullName.text = $"{user.FirstName} {user.LastName}";
    }

    private void ProfilePage()
    {
        var displayImage = profilePics.GetComponent<Image>();

        ProgressDialogue.Instance.SetLoadingCircleAnimation(loadingCircleAnimator, false);

        if (displayImage && UserDetail.Sprite)
            displayImage.sprite = UserDetail.Sprite;

        if (!profilePics.activeInHierarchy)
        {
            profilePics.SetActive(true);
        }

        loadingPanel.SetActive(false);
    }
}
