using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class StreamVideo : MonoBehaviour, ITrackableEventHandler
{


    //public VideoClip videoToPlay;
    public string video_url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


    private RawImage raw_image;
    private VideoPlayer video_player;
    private AudioSource audio_source;
    private TrackableBehaviour mTrackableBehaviour;
    private bool enable = false;

    void Awake()
    {
        Application.runInBackground = true;

        video_player = GetComponent<VideoPlayer>();
        audio_source = GetComponent<AudioSource>();
        raw_image = GetComponent<RawImage>();

        mTrackableBehaviour = transform.root.GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        raw_image.enabled = false;
    }


    public void Active(bool enable)
    {
        this.enable = enable;
        if (enable)
        {

            StartCoroutine(Prepare(false));
        }
        else
        {
            if (video_player != null)
                video_player.Stop();

            if (audio_source != null)
                audio_source.Stop();

        }

    }

    IEnumerator Prepare(bool play)
    {
        if (!video_player.isPrepared)
        {
            video_player.Prepare();
            while (!video_player.isPrepared)
                yield return null;

            raw_image.texture = video_player.texture;
        }

        if (play)
        {
            video_player.Play();
            audio_source.Play();
        }
    }



    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (!enable)
            return;

        Debug.Log("OnTrackableStateChanged " + newStatus);
        if (newStatus == TrackableBehaviour.Status.TRACKED)
        {

            raw_image.enabled = true;
            Debug.Log("video_player.isPrepared " + video_player.isPrepared);
            Debug.Log("video_player.isPlaying " + video_player.isPlaying);


            if (!video_player.isPlaying)
            {
                if (video_player.isPrepared)
                {
                    video_player.Play();
                    audio_source.Play();
                }
                else
                {
                    StartCoroutine(Prepare(true));
                }
            }

        }
        else
        {
            raw_image.enabled = false;

            if (video_player != null && video_player.isPlaying)
                video_player.Pause();

            if (audio_source != null && audio_source.isPlaying)
                audio_source.Pause();
        }
    }


#if false
	IEnumerator playVideo()
	{

		//Add VideoPlayer to the GameObject
		video_player = gameObject.AddComponent<VideoPlayer>();

		//Add AudioSource
		audio_source = gameObject.AddComponent<AudioSource>();

		//Disable Play on Awake for both Video and Audio
		video_player.playOnAwake = false;
		video_player.isLooping = true;
		audio_source.playOnAwake = false;
		audio_source.Pause();




		//Set Audio Output to AudioSource
		video_player.audioOutputMode = VideoAudioOutputMode.AudioSource;


		//Assign the Audio from Video to AudioSource to be played
		video_player.EnableAudioTrack(0, true);
		video_player.SetTargetAudioSource(0, audio_source);

		if (video_url == null || video_url.Length == 0)
		{
			video_player.source = VideoSource.VideoClip;
			//Set video To Play then prepare Audio to prevent Buffering
			video_player.clip = videoToPlay;
		}
		else
		{
			//We want to play from video clip not from url
			video_player.source = VideoSource.Url;
			video_player.url = video_url;
		}
		// Vide clip from Url
		//videoPlayer.source = VideoSource.Url;
		//videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";


		video_player.Prepare();

		//Wait until video is prepared
		while (!video_player.isPrepared)
		{
			yield return null;
		}

		Debug.Log("Done Preparing Video");

		//Assign the Texture from Video to RawImage to be displayed
		raw_image.texture = video_player.texture;

		//Play Video

		//video_player.time = start_sec;
		video_player.Play();


		//Play Sound
		audio_source.loop = true;
		audio_source.Play();


		Debug.Log("Playing Video");
		while (video_player.isPlaying)
		{
			//Debug.LogWarning("isPlaying Video Time: " + Mathf.FloorToInt((float)video_player.time));
			yield return null;
		}

		Debug.Log("Done Playing Video");
	}
#endif
}
