using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SharedResources;

public class ActivityPage : MonoBehaviour
{
    [SerializeField]
    private GameObject activityHeader;

    [SerializeField]
    private GameObject activityPrefab;

    [SerializeField]
    private Transform m_ContentContainer;

    private Button[] activityHeaderButtons;

    private void Awake()
    {
        activityHeaderButtons = activityHeader.GetComponentsInChildren<Button>();
    }
    void Start()
    {
        if (activityHeaderButtons != null && activityHeaderButtons.Any())
        {
            foreach (var button in activityHeaderButtons)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => ActivityHeaderClicked(button));
            }
        }

        GetActivity();
    }

    private void ActivityHeaderClicked(Button clickButton)
    {
        ColorUtility.TryParseHtmlString("#CFCFCF", out Color color);

        for (int i = 0; i < activityHeaderButtons.Length; i++)
        {
            var button = activityHeaderButtons[i];

            if (button != clickButton)
            {
                button.GetComponent<Image>().color = color;
                // Enable the button:
                button.interactable = true;
            }
        }

        clickButton.GetComponent<Image>().color = Color.white;

        // Disable the button:
        clickButton.interactable = false;
    }

    public void RefreshAvailableLevels(int childCount, Transform parent)
    {
        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void GetActivity()
    {
        int childCount = m_ContentContainer.childCount;

        RefreshAvailableLevels(childCount, m_ContentContainer);

        foreach (var activity in SharedResources.playerActivity)
        {
            var item_go = Instantiate(activityPrefab);
            item_go.transform.SetParent(m_ContentContainer);

            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            var image = item_go.GetComponentInChildren<Image>().transform;

            var profileImage = image.Find("ImageMask").GetComponent<Image>();

            _ = LoadTopicImageAsync(profileImage, activity.ProfilePicture);

            var textGameObject = item_go.GetComponentsInChildren<Transform>().First(c => !c.Find("Line"));

            var texts = textGameObject.GetComponentsInChildren<TextMeshProUGUI>();

            var name = texts.First(x => x.name == "Name");
            var activityObject = texts.First(x => x.name == "Activity");

            name.text = activity.Name;

            activityObject.text = activity.Activity;
        }
    }
}