using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;



public class VideoPlayback : MonoBehaviour
{

    public RawImage VideoPlayerRawImage;
    public Image playIcon;
    public Sprite iconImage;

    VideoClip videoToPlay;

    public VideoPlayer videoPlayer;
    private VideoSource videoSource;

    public AudioSource audioSource;

    private bool isPaused;
    private bool firstRun = true;

    public Sprite PlayImage;
    public Sprite PauseImage;


    IEnumerator playVideo()
    {

        firstRun = false;
        playIcon.sprite = PauseImage;

        ////Add VideoPlayer to the GameObject
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();

        ////Add AudioSource
        //audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();

        //We want to play from video clip not from url

        //videoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        //videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        // videoPlayer.clip = videoToPlay;
        if (!videoPlayer.isPrepared)
            videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        VideoPlayerRawImage.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();



        Debug.Log("Playing Video");
        while (videoPlayer.isPlaying)
        {
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }


        Debug.Log("Done Playing Video");




    }



    IEnumerator SetFirstFrame()
    {


        firstRun = true;
        isPaused = true;

        ////Add VideoPlayer to the GameObject
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();

        ////Add AudioSource
        //audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();
        playIcon.sprite = PlayImage;

        //We want to play from video clip not from url

        //videoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        //videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        // videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }


        //Assign the Texture from Video to RawImage to be displayed
        VideoPlayerRawImage.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();
        //Play Sound
        audioSource.Play();

        //Play Video
        //       videoPlayer.Pause();
        //Play Sound
        //     audioSource.Pause();
        playIcon.sprite = PlayImage;

        Debug.Log(" Video is ready");


    }


    IEnumerator PlayLoop()
    {

        ////Add VideoPlayer to the GameObject
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();

        ////Add AudioSource
        //audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();
        playIcon.sprite = PlayImage;

        //We want to play from video clip not from url

        //videoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        //videoPlayer.source = VideoSource.Url;
        //videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        // videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }


        //Assign the Texture from Video to RawImage to be displayed
        VideoPlayerRawImage.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();
        //Play Sound
        audioSource.Play();

        playIcon.sprite = PlayImage;

        Debug.Log(" Video is ready");

        while (videoPlayer.isPlaying)
        {
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }


        //Play Video
        videoPlayer.Stop();

        //Play Sound
        audioSource.Stop();

    }


    public void PlayPause()
    {
        if (!firstRun)
        {

            if (!isPaused)
            {
                videoPlayer.Pause();
                audioSource.Pause();
                // playIcon.enabled = true;
                playIcon.sprite = PlayImage;
                isPaused = true;

            }
            else
            {

                videoPlayer.Play();
                audioSource.Play();
                playIcon.sprite = PauseImage;
                //playIcon.enabled = false;
                isPaused = false;

            }
        }
        else
        {

            StartCoroutine(playVideo());

        }
    }

    string videoPath = "";

    public void PreparePlayback(string Path)
    {



        if (File.Exists(Path))
        {
            Debug.Log("File founded ");

            videoPath = Path;
            videoPlayer.url = Path;

            //   videoToPlay = videoPlayer.clip;
            StartCoroutine(playVideo());
            //if (videoToPlay)
            //  playIcon.sprite = iconImage;
            //  playIcon.sprite = Sprite.Create(videoPlayer, new Rect(0, 0, videoToPlay.width, videoToPlay.height), Vector2.zero);

        }
        else
        {
            Debug.Log("File not found");
        }
    }



    public void SaveVideoToGallery()
    {
        if (File.Exists(videoPath))
        {
            Debug.Log("Trying to save!");
            NativeGallery.SaveVideoToGallery(videoPath, "MentaAR", "MentaAR_Recording{0}.mp4", null);
            Invoke("DisplayMessage", 0.5f);
        }
    }

    void DisplayMessage()
    {
        MessageBox.DisplayMessageBox("Menta AR", "Video saved to gallery", true, null);
    }
}