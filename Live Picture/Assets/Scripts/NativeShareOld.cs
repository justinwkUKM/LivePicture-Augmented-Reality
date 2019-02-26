using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
public class NativeShareOld
{
    public string
        subject = "Menta AR Experience",
    url = "https://www.menta.live/",
    text = "Menta AR Experience\nFor more details please visit: https://www.menta.live/";


	private bool isProcessing = false;

	MonoBehaviour mono;

	public NativeShareOld(MonoBehaviour mono)
	{
		this.mono = mono;
	}


	/// <summary>
    /// Shares on file maximum
    /// </summary>
    /// <param name="body"></param>
    /// <param name="filePath">The path to the attached file</param>
    /// <param name="url"></param>
    /// <param name="subject"></param>
    /// <param name="mimeType"></param>
    /// <param name="chooser"></param>
    /// <param name="chooserText"></param>
    public void Share(string body, string filePath = null, string url = null, string subject = "", string mimeType = "text/html", bool chooser = false, string chooserText = "Select sharing app")
    {
        ShareMultiple(body, new string[] { filePath }, url, subject, mimeType, chooser, chooserText);
    }

    /// <summary>
    /// Shares multiple files at once
    /// </summary>
    /// <param name="body"></param>
    /// <param name="filePaths">The paths to the attached files</param>
    /// <param name="url"></param>
    /// <param name="subject"></param>
    /// <param name="mimeType"></param>
    /// <param name="chooser"></param>
    /// <param name="chooserText"></param>
    public void ShareMultiple(string body, string[] filePaths = null, string url = null, string subject = "", string mimeType = "text/html", bool chooser = false, string chooserText = "Select sharing app")
    {
#if UNITY_ANDROID
		ShareAndroid(body, subject, url, filePaths, mimeType, chooser, chooserText);
#elif UNITY_IOS
        mono.StartCoroutine(CallSocialShareRoutine());
#else
        Debug.Log("No sharing set up for this platform.");
        Debug.Log("Subject: " + subject);
        Debug.Log("Body: " + body);
#endif
    }

#if UNITY_ANDROID
	public void ShareAndroid(string body, string subject, string url, string[] filePaths, string mimeType, bool chooser, string chooserText)
	{
		using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
		using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
		{
			using (intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND")))
			{ }
			using (intentObject.Call<AndroidJavaObject>("setType", mimeType))
			{ }
			using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject))
			{ }
			using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body))
			{ }

			if (!string.IsNullOrEmpty(url))
			{
				// attach url
				using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
				using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url))
				using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject))
				{ }
			}
			else if (filePaths != null)
			{
				// attach extra files (pictures, pdf, etc.)
				using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
				using (AndroidJavaObject uris = new AndroidJavaObject("java.util.ArrayList"))
				{
					for (int i = 0; i < filePaths.Length; i++)
					{
						//instantiate the object Uri with the parse of the url's file
						using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + filePaths[i]))
						{
							uris.Call<bool>("add", uriObject);
						}
					}

					using (intentObject.Call<AndroidJavaObject>("putParcelableArrayListExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uris))
					{ }
				}
			}

			// finally start application
			using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				if (chooser)
                {
                    AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, chooserText);
                    currentActivity.Call("startActivity", jChooser);
                }
                else
                {
                    currentActivity.Call("startActivity", intentObject);
                }
			}
		}
	}
#endif


#if UNITY_IOS
    public struct ConfigStruct
    {
        public string title;
        public string message;
    }
    [DllImport("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
    public struct SocialSharingStruct
    {
        public string text;
        public string url;
        public string image;
        public string subject;
    }
    [DllImport("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
    public void CallSocialShare(string title, string message)
    {
        ConfigStruct conf = new ConfigStruct();
        conf.title = title;
        conf.message = message;
        showAlertMessage(ref conf);
        isProcessing = false;
    }
    public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
    {
        SocialSharingStruct conf = new SocialSharingStruct();
        conf.text = defaultTxt;
        conf.url = url;
        conf.image = img;
        conf.subject = subject;
        showSocialSharing(ref conf);
    }
    IEnumerator CallSocialShareRoutine()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();
        CallSocialShareAdvanced(text, subject, url, null);
    }
#endif

}
