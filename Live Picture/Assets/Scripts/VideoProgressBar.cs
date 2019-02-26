using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class VideoProgressBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{


    Slider progressBar;
    bool sliding;



    private void Awake()
    {
        progressBar = this.GetComponent<Slider>();
    }

    private void Update()
    {
        if (AppCore.instance.GetVideoPlayer() && AppCore.instance.GetVideoPlayer().isPlaying)
        {
            if (!sliding)
                if (AppCore.instance.GetVideoPlayer().frameCount > 0)
                    progressBar.value = (float)AppCore.instance.GetVideoPlayer().frame / (float)AppCore.instance.GetVideoPlayer().frameCount;

        }
        //        AppCore.instance.cloudRecoObj.targetScannerText.text = "Playing: " + (AppCore.instance.videoPlayerMain ? "Detected" : "Not detected");
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnDrag called");
    //    if (AppCore.instance.GetVideoPlayer() && AppCore.instance.GetVideoPlayer().isPlaying)
    //        TrySkip(eventData);
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerDown called");
    //    if (AppCore.instance.GetVideoPlayer() && AppCore.instance.GetVideoPlayer().isPlaying)
    //        TrySkip(eventData);
    //}

    //private void TrySkip(PointerEventData eventData)
    //{
    //    if (AppCore.instance.GetVideoPlayer() && AppCore.instance.GetVideoPlayer().isPlaying)
    //    {
    //        Vector2 localPoint;
    //        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            progressBar.rectTransform, eventData.position, null, out localPoint))
    //        {
    //            float pct = Mathf.InverseLerp(progressBar.rectTransform.rect.xMin, progressBar.rectTransform.rect.xMax, localPoint.x);
    //            SkipToPercent(pct);
    //        }
    //    }
    //}

    //private void SkipToPercent(float pct)
    //{
    //    if (AppCore.instance.GetVideoPlayer())
    //    {
    //        var frame = AppCore.instance.GetVideoPlayer().frameCount * pct;
    //        AppCore.instance.GetVideoPlayer().frame = (long)frame;
    //    }
    //}

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        sliding = true;

    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        AppCore.instance.cloudRecoObj.targetScannerText.text = "Paused video for seek";
        float frame = progressBar.value * AppCore.instance.GetVideoPlayer().frameCount;
        AppCore.instance.GetVideoPlayer().frame = (long)frame;
        Invoke("ResumeVideo", 1);
    }

    void ResumeVideo()
    {
        sliding = false;
        AppCore.instance.cloudRecoObj.targetScannerText.text = "Resumed video for seek";
    }
}