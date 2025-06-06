using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class UserSearch : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField searchInputField;

    [SerializeField]
    private GameObject userPrefab, friendPage;

    [SerializeField]
    private Transform m_ContentContainer;

    private string tempInput;
    private bool hasDisplayedUsers;
    private string searchParam;

    private float inputTimer, inputTimerInterval = 2f;

    private void OnDisable()
    {
        searchInputField.text = string.Empty;
    }

    // Update is called once per frame
    //void Updatee()
    //{
    //    inputTimerInterval -= Time.deltaTime;

    //    if(inputTimerInterval < 0f)
    //    {
    //        if (!string.IsNullOrWhiteSpace(tempInput) && tempInput.Equals(searchInputField.text, StringComparison.OrdinalIgnoreCase))
    //        {
    //            //make the call to fetch data
    //            Debug.Log("HITTING TESTINGGGG SEARCH");
    //            searchParam = searchInputField.text;
    //            _ = Task.Run(async () =>
    //            {
    //                var users = await ExternalService.SearchUsers("samuel1"/*UserDetail.UserId*/, searchParam);
    //                UserSearchDetails = users;

    //                MainThreadDispatcher.Enqueue(() =>
    //                {
    //                    LoadUserSearchImages();
    //                });
    //                hasDisplayedUsers = false;
    //            });
    //        }

    //        inputTimerInterval = 2f;
    //        searchInputField.text = tempInput = string.Empty;
    //    }
    //    else if (!string.IsNullOrEmpty(tempInput) && !tempInput.Equals(searchInputField.text, StringComparison.OrdinalIgnoreCase))
    //    {
    //        Debug.Log("RESETTING INTERVAL");
    //        inputTimerInterval = 2f;
    //    }

    //    tempInput = searchInputField.text;


    //    //if (!searchInputField.text.Equals(tempInput, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(searchInputField.text))
    //    //{
    //    //    Debug.LogWarning("HITTING UPDATE SEARCH");

    //    //    tempInput = searchInputField.text;

    //    //    _ = Task.Run(async () =>
    //    //    {
    //    //        var users = await ExternalService.SearchUsers("samuel1"/*UserDetail.UserId*/, searchInputField.text);
    //    //        UserSearchDetails = users;

    //    //        MainThreadDispatcher.Enqueue(() =>
    //    //        {
    //    //            LoadUserSearchImages();
    //    //        });
    //    //        hasDisplayedUsers = false;
    //    //    });
    //    //}
    //}

    public void OnChangeInputField()
    {
        if(string.IsNullOrWhiteSpace(searchInputField.text))
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            Debug.LogWarning($"SEARCH INPUT: {searchInputField.text}");
            var users = await ExternalService.SearchUsers(UserDetail.UserId, searchInputField.text);
            UserSearchDetails = users;

            MainThreadDispatcher.Enqueue(() =>
            {
                LoadUserSearchImages();
            });
            hasDisplayedUsers = false;
        });
    }

    private void LateUpdate()
    {
        if (UserSearchDetails != null && !UserSearchDetails.Exists(x => !x.IsImageLoadingStopped) && !hasDisplayedUsers)
        {
            hasDisplayedUsers = true;
            Debug.Log("MAPPING USERS");
            MapUsers();
        }
    }

    public void MapUsers()
    {
        int childCount = m_ContentContainer.childCount;

        RefreshAvailableLevels(childCount, m_ContentContainer);

        if (UserSearchDetails == null) return;

        foreach (var user in UserSearchDetails)
        {
            var item_go = Instantiate(userPrefab);
            item_go.transform.SetParent(m_ContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var image = item_go.GetComponentInChildren<Image>().transform;

            var profileImage = image.Find("ImageMask").GetComponent<Image>();

            profileImage.sprite = user.Sprite == null ? profileImage.sprite : user.Sprite;

            var texts = item_go.GetComponentsInChildren<TextMeshProUGUI>();

            var name = texts.First(x => x.name == "UserTextField");

            name.text = $"{user.FirstName} {user.LastName}";

            var button = item_go.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    var navSection = FindAnyObjectByType<NavigationSection>();

                    if (navSection == null)
                        return;

                    navSection.OnclickFriendInList(friendPage, user);
                });
            }
        }
    }

    public void RefreshAvailableLevels(int childCount, Transform parent)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}
