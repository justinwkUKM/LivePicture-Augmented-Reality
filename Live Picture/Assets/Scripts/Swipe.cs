using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {



    bool tap, swipeUp, swipeDown;

    Vector2 startTouch;
    bool isDragging;

    Vector2 _swipeDelta;

    public Vector2 SwipeDelta
    {
        get
        {
            return _swipeDelta;
        }

        set
        {
            _swipeDelta = value;
        }
    }

    public Vector2 StartTouch
    {
        get
        {
            return startTouch;
        }

        set
        {
            startTouch = value;
        }
    }

    public bool SwipeUp
    {
        get
        {
            return swipeUp;
        }

        set
        {
            swipeUp = value;
        }
    }


    // Use this for initialization
    void Start () {
		
	}


    // Update is called once per frame
    void Update()
    {
        tap = SwipeUp = swipeDown = false;





        if (Input.GetMouseButtonDown((0)))
        {

            tap = true;
            StartTouch = Input.mousePosition;
            //Debug.Log("Swiping");

        }
        else if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Swiping reset");

            Reset();
        }

        #region Mobile inputs

        if (Input.touches.Length != 0)
        {

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                isDragging = true;
                tap = true;

                StartTouch = Input.touches[0].position;
            }

            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDragging = false;
                Reset();
            }
        }

        #endregion


        // calculating the distance

        SwipeDelta = Vector2.zero;

        if (isDragging)
        {

            if (Input.touches.Length > 0)
                SwipeDelta = Input.touches[0].position - StartTouch;

            else if (Input.GetMouseButton(0))
                SwipeDelta = (Vector2)Input.mousePosition - StartTouch;
        }



        if (SwipeDelta.magnitude > 70)
        {

            float x = SwipeDelta.x;
            float y = SwipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                // x axes

            }
            else
            {
                // y axes
                if (y < 0)
                    swipeDown = true;
                else
                    SwipeUp = true;
            }
        }
    }


    void Reset(){

        StartTouch = SwipeDelta = Vector2.zero;

    }
}
