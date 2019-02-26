using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class VideoPlayerHandler : MonoBehaviour, ITrackableEventHandler
{

    public enum STATE
    {
        PLAYING,
        PREPARING,
        REDY,
        PAUSE,
        STOP
    }
    GameObject WindowedScreenRawImage;
    GameObject LoadingAnim;


    public VideoPlayer video_player;
    AudioSource audio_source;
    AppCore AppCoreObj;
    TrackableBehaviour mTrackableBehaviour;
    private TrackableBehaviour.Status status;


    public GameObject _playpauseBtn;
    public GameObject _fullScreenBtn;
    public GameObject _progressBar;

    public Action stop_play;
    public Action track_event;

    public STATE state = STATE.STOP;
    bool isFullScreen;


    public bool IsFullScreen
    {
        get
        {
            return isFullScreen;
        }

        set
        {
            isFullScreen = value;
        }
    }

    VideoClip video_clip;

    void Awake()
    {
        Application.runInBackground = true;
        AppCoreObj = GameObject.Find("AppCore").GetComponent<AppCore>();
        audio_source = video_player.GetComponent<AudioSource>();
        WindowedScreenRawImage = video_player.gameObject;
        LoadingAnim = video_player.transform.Find("LoadingAnim").gameObject;

        isFullScreen = false;
        AppCoreObj.SwitchScreenModeBtn.onClick.AddListener(SwitchScreenMode);
        AppCoreObj.PlayPauseBtn.onClick.AddListener(PlayPause);

        SwitchScreenMode(isFullScreen);
        CheckIconChange();
        mTrackableBehaviour = transform.root.GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }


    public void SetVideoClip(VideoClip clip)
    {
        video_clip = clip;
        //PrepareVideo();
    }




    public void SetVideoClipURL(string URL)
    {
        AppCoreObj.cloudRecoObj.targetScannerText.text = "SetVideoClip URL" + state;
        PrepareVideo(URL);
    }



    public void PrepareVideo()
    {

        if ((state == STATE.STOP) && video_clip != null) // removed the condition of null from
        {
            state = STATE.PREPARING;
            video_player.clip = video_clip;
            video_player.EnableAudioTrack(0, true);
            video_player.SetTargetAudioSource(0, audio_source);
            StartCoroutine(Prepare());
        }
    }


    private void PrepareVideo(string URL)
    {
        AppCoreObj.cloudRecoObj.targetScannerText.text = "PrepareVideo Main" + state;

        if (state == STATE.STOP) // removed the condition of video_clip != null
        {
            state = STATE.PREPARING;
            video_player.source = VideoSource.Url;
            video_player.url = URL;
            video_player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            video_player.controlledAudioTrackCount = 1;
            video_player.EnableAudioTrack(0, true);
            video_player.SetTargetAudioSource(0, audio_source);



            AppCoreObj.cloudRecoObj.targetScannerText.text = "Got inside stop state";
            StartCoroutine(Prepare());
        }
    }

    public void Stop()
    {
        if (state == STATE.PLAYING)
        {
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = false;
            AppCoreObj.FullScreenRawImage.SetActive(false);
            video_player.clip = null;

            if (video_player.isPlaying)
                video_player.Stop();

            if (audio_source.isPlaying)
                audio_source.Stop();

            state = STATE.STOP;
            video_clip = null;
            GC.Collect();
            CheckIconChange();

        }
    }

    IEnumerator Prepare()
    {
        AppCoreObj.cloudRecoObj.targetScannerText.text = "Got in preparing, Trying - State" + state;

        if (!video_player.isPrepared)
        {
            AppCoreObj.cloudRecoObj.targetScannerText.text = "Still preparing";

            video_player.Prepare();
            if (isFullScreen)
            {
                AppCoreObj.FullScreenRawImage.SetActive(true);
            }
            else
            {
                WindowedScreenRawImage.GetComponent<RawImage>().enabled = true;

            }

            while (!video_player.isPrepared)
            {
                LoadingAnim.SetActive(true);
                AppCoreObj.FullScreenLoadingAnim.enabled = true;
                yield return new WaitForSeconds(0.1f);

            }
            LoadingAnim.SetActive(false);
            _playpauseBtn.SetActive(true);
            _fullScreenBtn.SetActive(true);
            _progressBar.SetActive(true);
            AppCoreObj.FullScreenLoadingAnim.enabled = false;
            WindowedScreenRawImage.GetComponent<RawImage>().texture = video_player.texture;
            AppCoreObj.FullScreenRawImage.GetComponent<RawImage>().texture = video_player.texture;

            audio_source.priority = 0;
            AppCoreObj.cloudRecoObj.targetScannerText.text = "Preparing done";

            if (status == TrackableBehaviour.Status.TRACKED)
            {

                video_player.Play();
                audio_source.Play();
                state = STATE.PLAYING;
            }
            else
            {
                state = STATE.REDY;
                AppCoreObj.cloudRecoObj.targetScannerText.text = "Got in preparing, Not tracked";

            }
            CheckIconChange();

            Debug.Log("Prepare() State:[" + state + "]");
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status new_status)
    {

        Debug.Log("OnTrackableStateChange TrackableBehaviour.Status[ " + new_status + " ] STATE[ " + state + " ]");

        status = new_status;

        if (status == TrackableBehaviour.Status.DETECTED ||
            status == TrackableBehaviour.Status.TRACKED)
        {
            if (track_event != null)
                track_event();


            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }




    public void OnTrackingFound()
    {
        StartCoroutine("CheckVideoClip");

        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);
        //var audioComponents = GetComponentsInChildren<AudioSource>(true);


        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;


    }


    public void OnTrackingLost()
    {

        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);
        //var audioComponents = GetComponentsInChildren<AudioSource>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;

        //foreach (var component in audioComponents)
        //component.enabled = false;

        AppCoreObj.cloudRecoObj.targetScannerText.text = "Target lost: " + AppCoreObj.cloudRecoObj.TargetName;
        if (state == STATE.PLAYING)
        {
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = false;
            AppCoreObj.FullScreenRawImage.SetActive(false);
            if (video_player.isPlaying)
                video_player.Pause();

            if (audio_source.isPlaying)
                audio_source.Pause();

            _fullScreenBtn.SetActive(false);
            _playpauseBtn.SetActive(false);
            _progressBar.SetActive(false);
            state = STATE.PAUSE;
            AppCoreObj.EnableDisableIcons(false);
        }
    }


    IEnumerator CheckVideoClip()
    {
        AppCoreObj.cloudRecoObj.targetScannerText.text = "Got in CheckVideoClip";

        AppCoreObj.cloudRecoObj.targetScannerText.text = "Metadata :" + AppCoreObj.cloudRecoObj.TargetMetaData;
        SetVideoClipURL(AppCoreObj.cloudRecoObj.TargetMetaData["video_hyperlink"]);
        yield return new WaitForSeconds(0.5f);

        if (state == STATE.STOP)
        {
            if (video_clip)
                PrepareVideo();
        }

        if (state == STATE.PAUSE || state == STATE.REDY)
        {
            if (isFullScreen)
            {
                AppCoreObj.FullScreenRawImage.SetActive(true);
                WindowedScreenRawImage.GetComponent<RawImage>().enabled = false;
                _playpauseBtn.SetActive(false);
                _fullScreenBtn.SetActive(false);
                _progressBar.SetActive(false);
            }
            else
            {
                AppCoreObj.FullScreenRawImage.SetActive(false);
                WindowedScreenRawImage.GetComponent<RawImage>().enabled = true;
                _playpauseBtn.SetActive(true);
                _fullScreenBtn.SetActive(true);
                _progressBar.SetActive(true);

            }
            video_player.Play();
            audio_source.Play();
            state = STATE.PLAYING;
            AppCoreObj.EnableDisableIcons(true);
        }

    }


    private void Play()
    {
        if (isFullScreen)
        {
            AppCoreObj.FullScreenRawImage.SetActive(true);
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = false;
            _playpauseBtn.SetActive(false);
            _fullScreenBtn.SetActive(false);
            _progressBar.SetActive(false);

        }
        else
        {
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = true;
            AppCoreObj.FullScreenRawImage.SetActive(false);
            _playpauseBtn.SetActive(true);
            _fullScreenBtn.SetActive(true);
            _progressBar.SetActive(true);

        }
        video_player.Play();
        audio_source.Play();
        state = STATE.PLAYING;
    }


    void Update()
    {

        //Debug.Log(audio_source.time.ToString());

        if (state == STATE.PLAYING)
        {
            if (video_player.frame >= (long)video_player.frameCount)
            {
                Stop();

                if (stop_play != null)
                    stop_play();
            }
        }
    }



    Rect rect = LogHandler.scaleRectW(0.1f, 0f, 0.9f, 0.04f);
    GUIStyle style_log;

    //void OnGUI_()
    //{
    //    if (style_log == null)
    //    {
    //        style_log = LogHandler.defaultStyle(TextAnchor.UpperLeft, 0.025f, Color.white);
    //    }

    //    string log = "State[" + state + "] Status[" + status + "] Frame[" + video_player.frame + " / " + video_player.frameCount + "]";
    //    GUI.Box(rect, log, style_log);
    //}

    public void SwitchScreenMode()
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
        {
            AppCoreObj.FullScreenRawImage.SetActive(true);
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = false;
            AppCoreObj.FullScreenLoadingAnim.enabled = false;
            _playpauseBtn.SetActive(false);
            _fullScreenBtn.SetActive(false);
            _progressBar.SetActive(false);

        }
        else
        {
            AppCoreObj.FullScreenRawImage.SetActive(false);
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = true;
            _playpauseBtn.SetActive(true);
            _fullScreenBtn.SetActive(true); 
            _progressBar.SetActive(true);

        }
    }

    public void PlayPause()
    {
        if (video_player.isPlaying)
        {
            video_player.Pause();
        }
        else
        {
            video_player.Play();
        }
        CheckIconChange();
    }

    void SwitchScreenMode(bool isfullScreen)
    {
        AppCoreObj.FullScreenRawImage.SetActive(isfullScreen);
        if (video_player.isPlaying)
            WindowedScreenRawImage.GetComponent<RawImage>().enabled = !isfullScreen;
    }


    void CheckIconChange(){
        if(!video_player.isPrepared)
        {
            _playpauseBtn.GetComponent<UnityEngine.UI.Image>().sprite = AppCoreObj.stopBtnSprite;
        }else{
            _playpauseBtn.GetComponent<UnityEngine.UI.Image>().sprite = video_player.isPlaying ? AppCoreObj.playSprite : AppCoreObj.pauseSprite;

        }

        AppCoreObj.ChangeSprite(video_player);

    }
}
