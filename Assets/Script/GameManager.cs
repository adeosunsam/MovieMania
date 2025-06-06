using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string authToken;

    internal string token;

    private LoadingPanel loadingPanel;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        loadingPanel = Resources.FindObjectsOfTypeAll<LoadingPanel>().FirstOrDefault();
        token = authToken;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    internal void LoadingPanelInMainThread(bool isSuccessful = true, string message = "success", bool status = true)
    {
        MainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                //StartCoroutine(LoadPanelRoutine());
                if (loadingPanel == null)
                {
                    return;
                }

                if (!status)
                    ToastNotification.Show(message, isSuccessful ? "success" : "error");

                loadingPanel.gameObject.SetActive(status);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        });
    }

    private static IntPtr GetPlayerActivity()
    {
#if UNITY_ANDROID
        UnityEngine.AndroidJavaClass jc = new UnityEngine.AndroidJavaClass(
          "com.unity3d.player.UnityPlayer");
        return jc.GetStatic<UnityEngine.AndroidJavaObject>("currentActivity")
                 .GetRawObject();
#else
      return IntPtr.Zero;
#endif
    }
}
