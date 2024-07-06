using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityPage : MonoBehaviour
{
    [SerializeField]
    private GameObject activityHeader;

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
}
