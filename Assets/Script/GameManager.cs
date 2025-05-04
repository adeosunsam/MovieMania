using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //private void Start()
    //{
    //    Debug.Log($"GOOGLE SIGN IN CONFIGURATION: {GoogleSignIn.DefaultInstance != null}");

    //    GoogleSignIn.DefaultInstance.SignOut();
    //    GoogleSignIn.DefaultInstance.Disconnect();
    //}
    //[SerializeField]
    //private TextMeshProUGUI errorText;

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.V) || Input.anyKey)
    //    {
    //        Debug.Log("HITTING SPACE");
    //        ToastNotification.Show("Connection error\nCheck your internet connection and try again");
    //    }
    //}
    //void ShowToast(string text,
    //int duration)
    //{
    //    StartCoroutine(showToastCOR(text, duration));
    //}

    //private IEnumerator showToastCOR(string text,
    //    int duration)
    //{
    //    Color orginalColor = errorText.color;

    //    errorText.text = text;
    //    errorText.enabled = true;

    //    //Fade in
    //    yield return fadeInAndOut(errorText, true, 0.5f);

    //    //Wait for the duration
    //    float counter = 0;
    //    while (counter < duration)
    //    {
    //        counter += Time.deltaTime;
    //        yield return null;
    //    }

    //    //Fade out
    //    yield return fadeInAndOut(errorText, false, 0.5f);

    //    errorText.enabled = false;
    //    errorText.color = orginalColor;
    //}

    //IEnumerator fadeInAndOut(TextMeshProUGUI targetText, bool fadeIn, float duration)
    //{
    //    //Set Values depending on if fadeIn or fadeOut
    //    float a, b;
    //    if (fadeIn)
    //    {
    //        a = 0f;
    //        b = 1f;
    //    }
    //    else
    //    {
    //        a = 1f;
    //        b = 0f;
    //    }

    //    Color currentColor = Color.clear;
    //    float counter = 0f;

    //    while (counter < duration)
    //    {
    //        counter += Time.deltaTime;
    //        float alpha = Mathf.Lerp(a, b, counter / duration);

    //        targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    //        yield return null;
    //    }
    //}

    //public void ShowToast(string message)
    //{
    //    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    //    {
    //        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    //        if (activity != null)
    //        {
    //            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
    //            {
    //                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
    //                AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
    //                AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
    //                    "makeText", context, message, toastClass.GetStatic<int>("LENGTH_SHORT"));
    //                toast.Call("show");
    //            }));
    //        }
    //    }
    //    //#endif
    //}
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
