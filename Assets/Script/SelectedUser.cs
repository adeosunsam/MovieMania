using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RequestDtos;
using static SharedResources;

public class FriendPage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userName;

    [SerializeField]
    private Image userImage;

    [SerializeField]
    private Button followButton;

    void Awake()
    {
        NavigationSection.Instance.OnUserSelected += Instance_OnUserSelected;
    }

    private void Instance_OnUserSelected(object sender, UserDetailDto e)
    {
        MapUser(e);
    }

    private void OnApplicationQuit()
    {
        NavigationSection.Instance.OnUserSelected -= Instance_OnUserSelected;
    }
    // Update is called once per frame
    void Update()
    {

    }

    void MapUser(UserDetailDto userDetail)
    {
        userName.text = $"{userDetail.FirstName} {userDetail.LastName}";
        userImage.sprite = userDetail.Sprite;

        if(followButton != null )
        {
            if(UserFriends.Exists(x => x.UserId == userDetail.UserId))
            {
                followButton.interactable = false;
            }
            followButton.onClick.RemoveAllListeners();
            followButton.onClick.AddListener(() =>
            {
                _ = Task.Run(async () =>
                {
                    GameManager.Instance.LoadingPanelInMainThread();
                    var response = await ExternalService.FollowUserRequest(new UserFollowRequest
                    {
                        UserId = UserDetail.UserId,
                        FriendId = userDetail.UserId
                    });

                    if (response.Data)
                    {
                        BroadcastService.Singleton.SendActivitiesAsync(userDetail.UserId);

                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: true, message: response.ResponseMessage, status: false);
                    }
                    else
                    {
                        GameManager.Instance.LoadingPanelInMainThread(isSuccessful: false, message: response.ResponseMessage, status: false);
                    }
                });
            });
        }
    }
}
