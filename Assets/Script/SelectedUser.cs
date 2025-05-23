using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RequestDtos;

public class FriendPage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userName;

    [SerializeField]
    private Image userImage;

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
    }
}
