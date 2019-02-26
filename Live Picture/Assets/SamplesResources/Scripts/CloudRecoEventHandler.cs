/*==============================================================================
Copyright (c) 2015-2017 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/
using System;
using UnityEngine;
using Vuforia;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecoEventHandler : MonoBehaviour, ICloudRecoEventHandler
{
    #region PRIVATE_MEMBERS
    // CloudRecoBehaviour reference to avoid lookups
    private CloudRecoBehaviour mCloudRecoBehaviour;
    // ObjectTracker reference to avoid lookups
    private ObjectTracker mObjectTracker;
    // reference to the cloud reco scene manager component:
    private ContentManager mContentManager;

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
    public ScanLine scanLine;
    /// <summary>
    /// Reference to UI Canvas to show Cloud Reco errors.
    /// </summary>
    public Canvas cloudErrorCanvas;
    public UnityEngine.UI.Text cloudErrorTitle;
    public UnityEngine.UI.Text cloudErrorText;
    public UnityEngine.UI.Image cloudActivityIcon;
    #endregion //PUBLIC_VARIABLES


    #region ICloudRecoEventHandler_IMPLEMENTATION
    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized(TargetFinder targetFinder)
    {
        // get a reference to the Object Tracker, remember it
        mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        mContentManager = FindObjectOfType<ContentManager>();
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

        ShowError(errorTitle, errorMsg);
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

        ShowError(errorTitle, errorMsg);
    }

    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
    /// </summary>
    public void OnStateChanged(bool scanning)
    {
        if (scanning)
        {
            // clear all known trackables
            mObjectTracker.TargetFinder.ClearTrackables(false);

            // hide the ImageTargetTemplate
            mContentManager.HideObject();
        }

        ShowScanLine(scanning);
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
        //if (targetSearchResult.TargetName != "")
        //{
        //    return;
        //}
        if (targetSearchResult.TargetName.Length > 1)
        {
            errorTitle = "Detected";
            errorMsg = "Marker detected: " + targetSearchResult.TargetName;
            ShowError(errorTitle, errorMsg);
        }
        // Enable the new result with the same ImageTargetBehaviour:
        ImageTargetBehaviour imageTargetBehaviour = mObjectTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate) as ImageTargetBehaviour;

        if (imageTargetBehaviour != null)
        {
            // Stop the target finder
            mCloudRecoBehaviour.CloudRecoEnabled = false;

            // Stop showing the scan-line
            ShowScanLine(false);

            // Calls the TargetCreated Method of the SceneManager object to start loading
            // the BookData from the JSON
            mContentManager.TargetCreated(targetSearchResult.MetaData);
            mContentManager.AnimationsManager.SetInitialAnimationFlags();

        }

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
            ShowError(errorTitle, errorMsg);
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
        SetCloudActivityIconVisible(false);
    }

    void Update()
    {
        if (mCloudRecoBehaviour.CloudRecoInitialized)
        {
            // Show/hide the requesting message panel
            //SetCloudActivityIconVisible(mObjectTracker.TargetFinder.IsRequesting());
			SetCloudActivityIconVisible(mObjectTracker.TargetFinder.IsRequesting());

        }
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void CloseErrorDialog()
    {
        if (cloudErrorCanvas)
        {
            cloudErrorCanvas.transform.parent.position = Vector3.right * 2 * Screen.width;
            cloudErrorCanvas.gameObject.SetActive(false);
            cloudErrorCanvas.enabled = false;

            if (mMustRestartApp)
            {
                mMustRestartApp = false;
                RestartApplication();
            }
        }
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void ShowScanLine(bool show)
    {
        // Toggle scanline rendering
        if (scanLine != null)
        {
            Renderer scanLineRenderer = scanLine.GetComponent<Renderer>();
            if (show)
            {
                // Enable scan line rendering
                if (!scanLineRenderer.enabled)
                    scanLineRenderer.enabled = true;

                scanLine.ResetAnimation();
            }
            else
            {
                // Disable scanline rendering
                if (scanLineRenderer.enabled)
                    scanLineRenderer.enabled = false;
            }
        }
    }

    private void SetCloudActivityIconVisible(bool visible)
    {
        if (!cloudActivityIcon) return;

        if (visible != cloudActivityIcon.enabled)
            cloudActivityIcon.enabled = visible;
    }

    private void ShowError(string title, string msg)
    {
        if (!cloudErrorCanvas) return;

        if (cloudErrorTitle)
            cloudErrorTitle.text = title;

        if (cloudErrorText)
            cloudErrorText.text = msg;

        // Show the error canvas
        cloudErrorCanvas.transform.parent.position = Vector3.zero;
        cloudErrorCanvas.gameObject.SetActive(true);
        cloudErrorCanvas.enabled = true;
    }

    // Error Handling Callback that gets called when the application is not connected to the internet
    private void RestartApplication()
    {
        //Restarts the app
        int startLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
        if (startLevel < 0) startLevel = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(startLevel);
    }
    #endregion // PRIVATE_METHODS
}
