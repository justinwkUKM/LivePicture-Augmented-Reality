/*============================================================================== 
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
 * ==============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

/// <summary>
/// This class manages the content displayed on top of cloud reco targets in this sample
/// </summary>
public class ContentManager : MonoBehaviour, ITrackableEventHandler
{
    #region PUBLIC_VARIABLES
    public UnityEngine.UI.Image LoadingSpinnerBackground;
    public UnityEngine.UI.Image LoadingSpinnerImage;
    public Button CancelButton;

    /// <summary>
    /// The root gameobject that serves as an augmentation for the image targets created by search results
    /// </summary>
    public GameObject AugmentationObject;

    /// <summary>
    /// Reference to the script handling animations between 2D and 3D.
    /// </summary>
    public AnimationsManager AnimationsManager;

    /// <summary>
    /// the URL the JSON data should be fetched from
    /// </summary>
    public string JsonServerUrl;
    #endregion //PUBLIC_VARIABLES


    #region PRIVATE_MEMBERS
    private bool mIsShowingBookData = false;
    private bool mIsLoadingBookData = false;
    private bool mIsLoadingBookThumb = false;
    private WWW mJsonBookInfo;
    private WWW mBookThumb;
    private BookData mBookData;
    private bool mIsBookThumbRequested = false;
    private BookInformationParser mBookInformationParser;
    private bool mIsShowingMenu = false;
    private CloudRecoBehaviour mCloudRecoBehaviour;
    private TrackableBehaviour mTrackableBehaviour;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        // setup BookInformationParser 
        mBookInformationParser = new BookInformationParser();
        mBookInformationParser.SetBookObject(AugmentationObject);

        mTrackableBehaviour = AugmentationObject.transform.parent.GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        mCloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();

        HideObject();

        SetCancelButtonVisible(false);
    }

    void Update()
    {
        if (mIsShowingBookData)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000.0f))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject != null && hitObject.name == "BookInformation")
                    {
                        if (mBookData != null && mIsShowingMenu == false)
                        {
                            Application.OpenURL(mBookData.BookDetailUrl);
                        }
                    }
                }
            }
        }

        if (mIsLoadingBookThumb)
        {
            LoadBookThumb();
        }

        // Show/hide loading progress spinner if we are loading book data or thumb
        SetLoadingSpinnerVisible(mIsLoadingBookData || mIsLoadingBookThumb);

        // Show cancel button if the Cloud Reco is not enabled, otherwise hide it
        SetCancelButtonVisible(mCloudRecoBehaviour.CloudRecoInitialized && !mCloudRecoBehaviour.CloudRecoEnabled);
    }
    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void OnCancel()
    {
        mCloudRecoBehaviour.CloudRecoEnabled = true;
        TargetDeleted();
    }

    /// <summary>
    /// Method called from the CloudRecoEventHandler
    /// when a new target is created
    /// </summary>
    public void TargetCreated(string targetMetadata)
    {
        // Initialize the showing book data variable
        mIsShowingBookData = true;
        mIsLoadingBookData = true;

        mIsBookThumbRequested = false;

        // Loads the JSON Book Data
        StartCoroutine(LoadJSONBookData(targetMetadata));
    }

    /// <summary>
    /// Method called when the Close button is pressed
    /// to clean the target Data
    /// </summary>
    public void TargetDeleted()
    {
        // Initialize the showing book data variable
        mIsShowingBookData = false;
        mIsLoadingBookData = false;
        mIsLoadingBookThumb = false;
        mBookData = null;
    }

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    /// <summary>
    /// Hides the augmentation object
    /// </summary>
    public void HideObject()
    {
        Renderer[] rendererComponents = AugmentationObject.GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = AugmentationObject.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }
    }

    /// <summary>
    /// Method to let the ContentManager know if the CloudReco
    /// SampleMenu is being displayed
    /// </summary>
    public void SetIsShowingMenu(bool isShowing)
    {
        mIsShowingMenu = isShowing;
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void SetLoadingSpinnerVisible(bool visible)
    {
        if (LoadingSpinnerBackground == null ||
            LoadingSpinnerImage == null)
            return;

        if (LoadingSpinnerBackground.enabled != visible)
            LoadingSpinnerBackground.enabled = visible;

        if (LoadingSpinnerImage.enabled != visible)
            LoadingSpinnerImage.enabled = visible;

        if (visible)
        {
            LoadingSpinnerImage.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
        }
    }

    private void SetCancelButtonVisible(bool visible)
    {
        if (CancelButton == null) return;

        if (CancelButton.enabled != visible)
        {
            CancelButton.enabled = visible;
            CancelButton.image.enabled = visible;
        }
    }

    /// <summary>
    /// Method called from the CloudReco Trackable Event Handler
    /// when a target has been found
    /// </summary>
    private void OnTrackingFound()
    {
        // Checks tha the book info is displayed
        if (mIsShowingBookData)
        {
            // Starts playing the animation to 3D
            AnimationsManager.PlayAnimationTo3D(AugmentationObject);
        }
    }

    /// <summary>
    /// Method called from the CloudReco Trackable Event Handler
    /// when a target has been Lost
    /// </summary>
    private void OnTrackingLost()
    {
        // Checks tha the book info is displayed
        if (mIsShowingBookData)
        {
            // Starts playing the animation to 2D
            AnimationsManager.PlayAnimationTo2D(AugmentationObject);
        }
    }

    /// <summary>
    /// Fetches the JSON data from a server
    /// </summary>
    private IEnumerator LoadJSONBookData(string jsonBookUrl)
    {
        // Gets the full book json url
        string fullBookURL = JsonServerUrl + jsonBookUrl;

        // Gets the json book info from the url
        mJsonBookInfo = new WWW(fullBookURL);
        yield return mJsonBookInfo;

        // Loading done
        mIsLoadingBookData = false;

        if (mJsonBookInfo.error == null)
        {
            // Parses the json Object
            JSONParser parser = new JSONParser();

            BookData bookData = parser.ParseString(mJsonBookInfo.text);
            mBookData = bookData;

            // Updates the BookData info in the augmented object
            mBookInformationParser.UpdateBookData(bookData);

            mIsLoadingBookThumb = true;
        }
        else
        {
            Debug.LogError("Error downloading json");
        }
    }

    /// <summary>
    /// Loads the texture for the book thumbnail
    /// </summary>
    private void LoadBookThumb()
    {
        if (!mIsBookThumbRequested)
        {
            if (mBookData != null)
            {
                mBookThumb = new WWW(mBookData.BookThumbUrl);
                mIsBookThumbRequested = true;
            }
        }

        if (mBookThumb.progress >= 1)
        {
            if (mBookThumb.error == null && mBookData != null)
            {
                mBookInformationParser.UpdateBookThumb(mBookThumb.texture);
                mIsLoadingBookThumb = false;
                ShowObject();
            }
        }
    }

    /// <summary>
    /// Shows the augmentation object
    /// </summary>
    private void ShowObject()
    {
        Renderer[] rendererComponents = AugmentationObject.GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = AugmentationObject.GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }
    }
    #endregion //PRIVATE_METHODS
}
