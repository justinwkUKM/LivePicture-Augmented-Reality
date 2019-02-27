using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.Video;
using Vuforia;
using LitJson;
using System.Linq;

public class AppCore : MonoBehaviour//, IDragHandler, IPointerDownHandler
{

    #region Variables


    VideoPlayer videoPlayerMain;
    public Camera cameraMain;
    #endregion

    public CustomCloudRecoEventHandler cloudRecoObj;
    public GameObject FullScreenRawImage;
    public UnityEngine.UI.Image FullScreenLoadingAnim;
    public Button SwitchScreenModeBtn;
    public Button PlayPauseBtn;
    public static bool midAirObjectIsPlaced;
    public Sprite pauseSprite;
    public Sprite playSprite;
    public Sprite stopBtnSprite;
    public static AppCore instance;

    public GameObject banner;
    public Sprite[] imagesToSwap;
    public float swapTime = 3f;
    UnityEngine.UI.Image BannerImage;
    bool canChange = true;

    public UnityEngine.UI.Image[] IconImages;


    void Awake()
    {
        instance = this;
        BannerImage = banner.GetComponent<UnityEngine.UI.Image>();
        BannerImage.sprite = imagesToSwap[0];
    }


    void Start()
    {
        VuforiaSetup(true);
        InvokeRepeating("Swap", 2f, swapTime);
        EnableDisableIcons(false);
    }


    private bool preparing = false;
    private void VuforiaSetup(bool enable)
    {
        if (preparing)
        {
            Debug.Log("VuforiaSetup: preparing. return;");
            return;
        }

        preparing = true;
        StartCoroutine(VuforiaActive(enable));
    }


    private bool next = false;


    private IEnumerator VuforiaActive(bool enable)
    {

        //Debug.Log("VuforiaActive: [" + enable + "]");

        if (enable)
        {

            Vuforia.CameraDevice.Instance.Start();

            while (!Vuforia.CameraDevice.Instance.IsActive())
            {
                yield return new WaitForSeconds(0.1f);
            }

            Vuforia.ObjectTracker vuforia_object_tracker = Vuforia.TrackerManager.Instance.GetTracker<Vuforia.ObjectTracker>();

            if (vuforia_object_tracker != null)
            {
                vuforia_object_tracker.Start();
                while (!vuforia_object_tracker.IsActive)
                    yield return new WaitForSeconds(0.1f);
            }
            preparing = false;





        }
        else
        {
            //stream_video.Active(false);



            //	Debug.Log("player_handler.Stop();");

            Vuforia.ObjectTracker vuforia_object_tracker = TrackerManager.Instance.GetTracker<Vuforia.ObjectTracker>();

            if (vuforia_object_tracker != null)
            {
                vuforia_object_tracker.Stop();
                while (vuforia_object_tracker.IsActive)
                    yield return new WaitForSeconds(0.1f);
            }

            Vuforia.CameraDevice.Instance.Stop();
            while (Vuforia.CameraDevice.Instance.IsActive())
                yield return new WaitForSeconds(0.1f);

            preparing = false;
        }
    }

    TrackableBehaviour ActiveImageTarget;


    public void SetVideoPlayer(VideoPlayer _vPlayer)
    {
        videoPlayerMain = _vPlayer;
    }

    public void RemoveVideoPlayer()
    {
        videoPlayerMain = null;
    }



    public VideoPlayer GetVideoPlayer()
    {
        return videoPlayerMain;
    }


    public void ChangeSprite(VideoPlayer _vplayer)
    {
        if (!_vplayer.isPrepared)
        {
            PlayPauseBtn.GetComponent<UnityEngine.UI.Image>().sprite = stopBtnSprite;
        }
        else
        {
            PlayPauseBtn.GetComponent<UnityEngine.UI.Image>().sprite = _vplayer.isPlaying ? playSprite : pauseSprite;
        }
    }

    public void Swap()
    {
        int i = UnityEngine.Random.Range(0, imagesToSwap.Length);
        BannerImage.sprite = imagesToSwap[i];

    }


    public void EnableDisableIcons(bool val)
    {
        if (val)
        {
            string[] _dictionaryKeys = cloudRecoObj.TargetMetaData.Keys.ToArray();

            for (int i = 0; i < _dictionaryKeys.Length; i++)
            {
                for (int j = 0; j < IconImages.Length; j++)
                {
                    IconImages[j].enabled = !(cloudRecoObj.TargetMetaData[IconImages[j].name] == null);
                }
            }
        }
        else
        {
            for (int i = 0; i < IconImages.Length; i++)
            {
                IconImages[i].enabled = val;
            }
        }
    }


    public void OpenRedirectURL() {

        Debug.Log("SelectedGameobject is : " + EventSystem.current.currentSelectedGameObject.name);
        Application.OpenURL(cloudRecoObj.TargetMetaData[EventSystem.current.currentSelectedGameObject.name]);
    }
}

