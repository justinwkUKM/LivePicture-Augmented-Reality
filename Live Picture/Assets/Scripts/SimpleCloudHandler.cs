


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{

    public ImageTargetBehaviour ImageTargetTemplate;
    CloudRecoBehaviour cloudRecoBehaviour;
    bool isScanning;
    public Text targetScannerText;
    string targetName = "";
    string _targetMetaData;
    public AppCore AppCoreObj;


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

    public string TargetMetaData
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

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.LogError("Cloud Reco failed to initialize!");
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log("Cloud Reco initialized");
   
    }

    public void OnInitialized()
    {
        Debug.Log("Cloud Reco initialized");

    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        GameObject newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
        GameObject augmentation = null;
        if (augmentation != null)
            augmentation.transform.SetParent(newImageTarget.transform);

        TargetName = targetSearchResult.TargetName;
        TargetMetaData = targetSearchResult.MetaData;
        if (newImageTarget)
        {
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);
        }


        if (!isScanning)
        {
            cloudRecoBehaviour.CloudRecoEnabled = true;
        }

        if (targetSearchResult.MetaData != "")
        {
            targetScannerText.text = targetSearchResult.MetaData;
        }
        else
        {
            targetScannerText.text = targetSearchResult.TargetName;
        }
    }


    public void OnStateChanged(bool scanning)
    {
        isScanning = scanning;

        if (isScanning)
        {
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);
            targetScannerText.text = "Scanning";
        }
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
    }

    // Use this for initialization
    void Start()
    {

        CloudRecoBehaviour _cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (_cloudRecoBehaviour)
        {
            _cloudRecoBehaviour.RegisterEventHandler(this);
        }
        cloudRecoBehaviour = _cloudRecoBehaviour;
    }
}





//using System;
//using UnityEngine;
//using System.Collections;
//using Vuforia;

//public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
//{
//    public ImageTargetBehaviour ImageTargetTemplate;
//    private CloudRecoBehaviour mCloudRecoBehaviour;
//    private bool mIsScanning = false;
//    private string mTargetMetadata = "";
//    // Use this for initialization
//    void Start()
//    {
//        // register this event handler at the cloud reco behaviour
//        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
//        if (mCloudRecoBehaviour)
//        {
//            mCloudRecoBehaviour.RegisterEventHandler(this);
//        }
//    }

//    public void OnInitialized()
//    {
//        Debug.Log("Cloud Reco initialized");
//    }
//    public void OnInitError(TargetFinder.InitState initError)
//    {
//        Debug.Log("Cloud Reco init error " + initError.ToString());
//    }
//    public void OnUpdateError(TargetFinder.UpdateState updateError)
//    {
//        Debug.Log("Cloud Reco update error " + updateError.ToString());
//    }

//    public void OnStateChanged(bool scanning)

//    {
//        mIsScanning = scanning;
//        if (scanning)
//        {
//            // clear all known trackables
//            //ImageTracker tracker = TrackerManager.Instance.GetTracker<ImageTracker>();
//            //tracker.TargetFinder.ClearTrackables(false);
//        }
//    }
//    // Here we handle a cloud target recognition event
//    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
//    {
//        // do something with the target metadata
//        mTargetMetadata = targetSearchResult.TargetName;
//        // stop the target finder (i.e. stop scanning the cloud)
//        mCloudRecoBehaviour.CloudRecoEnabled = false;
//        // Build augmentation based on target
//        if (ImageTargetTemplate)
//        {
//            // enable the new result with the same ImageTargetBehaviour:
//            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
//            ImageTargetBehaviour imageTargetBehaviour =
//                (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(
//                    targetSearchResult, ImageTargetTemplate.gameObject);
//        }
//    }

//    void OnGUI()
//    {
//        // Display current 'scanning' status
//        GUI.Box(new Rect(100, 100, 200, 50), mIsScanning ? "Scanning" : "Not scanning");
//        // Display metadata of latest detected cloud-target
//        GUI.Box(new Rect(100, 200, 200, 50), "Metadata: " + mTargetMetadata);
//        // If not scanning, show button
//        // so that user can restart cloud scanning
//        if (!mIsScanning)
//        {
//            if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
//            {
//                // Restart TargetFinder
//                mCloudRecoBehaviour.CloudRecoEnabled = true;
//            }
//        }
//    }
//}

