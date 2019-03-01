/*==============================================================================
Copyright (c) 2015-2017 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/
using System;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Linq;
using LitJson;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CustomCloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{
    
    #region PRIVATE_MEMBERS
    // CloudRecoBehaviour reference to avoid lookups
    private CloudRecoBehaviour mCloudRecoBehaviour;

    // ObjectTracker reference to avoid lookups
    private ObjectTracker mObjectTracker;


    // the parent gameobject of the referenced ImageTargetTemplate - reused for all target search results
    private GameObject mParentOfImageTargetTemplate;
    private bool mMustRestartApp = false;
    private string errorTitle;
    private string errorMsg;
    #endregion // PRIVATE_MEMBERS


    #region PUBLIC_VARIABLES
    /// <summary>
    /// Can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;
    /// <summary>
    /// The scan-line rendered in overlay when Cloud Reco is in scanning mode.
    /// </summary>
    //public ScanLine scanLine;
    /// <summary>
    /// Reference to UI Canvas to show Cloud Reco errors.
    /// </summary>
    //public Canvas cloudErrorCanvas;
    //public UnityEngine.UI.Text cloudErrorTitle;
    //public UnityEngine.UI.Text cloudErrorText;
    //public UnityEngine.UI.Image cloudActivityIcon;
    #endregion //PUBLIC_VARIABLES


    public AppCore CoreObj;
    public Text targetScannerText;

    VideoPlayerHandler Cloud_Video_Player;

    string targetName = "";

    /*
     * 
     * 
     * {"video_hyperlink":"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
     * "redirect_url":"www.facebook.com",
     * "facebook":"www.facebook.com",
     * "instagram":"www.facebook.com",
     * "whatsapp":"www.facebook.com",
     * "linkedln":"www.facebook.com",
     * "twitter":"www.facebook.com",
     * "youtube":"www.facebook.com",
     * "maps_location":null,
     * "website":"www.facebook.com"}
*/




    Dictionary<string, string> _targetMetaData = new Dictionary<string, string>() {
        {"video_hyperlink","" },
        {"redirect_url","" },
        {"facebook","" },
        {"instagram","" },
        {"whatsapp","" },
        { "twitter","" },
        {"linkedln","" },
        {"youtube","" },
        {"maps_location","" },
        {"website","" }
    };

    public string TargetName
    {
        get
        {
            return targetName;
        }

        set
        {
            targetName = value;
        }
    }

    public Dictionary<string, string> TargetMetaData
    {
        get
        {
            return _targetMetaData;
        }

        set
        {
            _targetMetaData = value;
        }
    }

    #region ICloudRecoEventHandler_IMPLEMENTATION
    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized()
    {
        // get a reference to the Object Tracker, remember it
        mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        targetScannerText.text = "Cloud Reco - Initialized";

    }


    /// <summary>
    /// visualize initialization errors
    /// </summary>
    public void OnInitError(TargetFinder.InitState initError)
    {
        switch (initError)
        {
            case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                mMustRestartApp = true;
                errorTitle = "Network Unavailable";
                errorMsg = "Please check your Internet connection and try again.";
                break;
            case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                errorTitle = "Service Unavailable";
                errorMsg = "Failed to initialize app because the service is not available.";
                break;
        }

        // Prepend the error code in red
        errorMsg = "<color=red>" + initError.ToString().Replace("_", " ") + "</color>\n\n" + errorMsg;

        // Remove rich text tags for console logging
        var errorTextConsole = errorMsg.Replace("<color=red>", "").Replace("</color>", "");

        Debug.LogError("Cloud Reco - Initialization Error: " + initError + "\n\n" + errorTextConsole);
        targetScannerText.text = "Cloud Reco - Update Error: " + initError + "\n\n" + errorTextConsole;

        //ShowError(errorTitle, errorMsg);
    }


    /// <summary>
    /// visualize update errors
    /// </summary>
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        switch (updateError)
        {
            case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
                errorTitle = "Authorization Error";
                errorMsg = "The cloud recognition service access keys are incorrect or have expired.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
                errorTitle = "Network Unavailable";
                errorMsg = "Please check your Internet connection and try again.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
                errorTitle = "Authorization Error";
                errorMsg = "The cloud recognition service has been suspended.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
                errorTitle = "Request Timeout";
                errorMsg = "The network request has timed out, please check your Internet connection and try again.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
                errorTitle = "Service Unavailable";
                errorMsg = "The service is unavailable, please try again later.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
                errorTitle = "Clock Sync Error";
                errorMsg = "Please update the date and time and try again.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
                errorTitle = "Unsupported Version";
                errorMsg = "The application is using an unsupported version of Vuforia.";
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_BAD_FRAME_QUALITY:
                errorTitle = "Bad Frame Quality";
                errorMsg = "Low-frame quality has been continuously observed.\n\nError Event Received on Frame: " + Time.frameCount;
                break;
        }
        

        // Prepend the error code in red
        errorMsg = "<color=red>" + updateError.ToString().Replace("_", " ") + "</color>\n\n" + errorMsg;

        // Remove rich text tags for console logging
        var errorTextConsole = errorMsg.Replace("<color=red>", "").Replace("</color>", "");

        Debug.LogError("Cloud Reco - Update Error: " + updateError + "\n\n" + errorTextConsole);
        targetScannerText.text = "Cloud Reco - Update Error: " + updateError + "\n\n" + errorTextConsole;

        //ShowError(errorTitle, errorMsg);
    }

    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
    /// </summary>
    public void OnStateChanged(bool scanning)
    {


        if (scanning)
        {
            // clear all known trackables
            //mObjectTracker.TargetFinder.ClearTrackables(false);

            if (Cloud_Video_Player)
                if (Cloud_Video_Player.state == VideoPlayerHandler.STATE.PLAYING)
                {
                    Cloud_Video_Player.Stop();
                    CoreObj.RemoveVideoPlayer();
                }
        }
    }





    /// <summary>
    /// Handles new search results
    /// </summary>
    /// <param name="targetSearchResult"></param>
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        // This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
        // Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
        // or to create a new ImageTargetBehaviour for each new result

        // Vuforia will return a new object with the right script automatically if you use
        // TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)

        // Check if the metadata isn't null

        TargetName = targetSearchResult.TargetName;
        string[] _dictionaryKeys = TargetMetaData.Keys.ToArray();

        JsonData jsonValue = JsonMapper.ToObject(targetSearchResult.MetaData);
        targetScannerText.text = "Found: " + targetSearchResult.TargetName;
/*
        foreach (string s in _dictionaryKeys)
        {
            if (jsonValue.Keys.Contains(s))
                TargetMetaData[s] = jsonValue[s].ToString();
        }


        //WindowedScreenTransform

        GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
        Cloud_Video_Player = newImageTarget.transform.GetComponentInChildren<VideoPlayerHandler>();
        CoreObj.SetVideoPlayer(Cloud_Video_Player.video_player);
        CoreObj.EnableDisableIcons(true);



        GameObject augmentation = null;
        if (augmentation != null)
            augmentation.transform.SetParent(newImageTarget.transform);


        targetScannerText.text = "Scanned: " + targetSearchResult.TargetName;

        if (newImageTarget)
        {
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);
        }
        */
    }
    #endregion // ICloudRecoEventHandler_IMPLEMENTATION


    #region MONOBEHAVIOUR_METHODS
    /// <summary>
    /// Register for events at the CloudRecoBehaviour
    /// </summary>
    void Start()
    {
        if (VuforiaConfiguration.Instance.Vuforia.LicenseKey == string.Empty)
        {
            errorTitle = "Cloud Reco Init Error";
            errorMsg = "Vuforia License Key not found. Cloud Reco requires a valid license.";
            //ShowError(errorTitle, errorMsg);
        }

        // Look up the gameobject containing the ImageTargetTemplate:
        mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;

        // Register this event handler at the cloud reco behaviour
        CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (cloudRecoBehaviour)
        {
            cloudRecoBehaviour.RegisterEventHandler(this);
        }

        // Remember cloudRecoBehaviour for later
        mCloudRecoBehaviour = cloudRecoBehaviour;

        // At start we hide the requesting message panel
        //SetCloudActivityIconVisible(false);
    }



    #endregion //MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    // Error Handling Callback that gets called when the application is not connected to the internet
    private void RestartApplication()
    {
        //Restarts the app
        int startLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
        if (startLevel < 0) startLevel = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(startLevel);
    }





    private void Awake()
    {
        targetScannerText.text = "";
    }
    #endregion // PRIVATE_METHODS
}
