using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptCameraRotation : MonoBehaviour {





    bool _isPressed;
    bool _isHolding;
    float _currentTime;

    float _holdingTime = 0.5f;

    public bool IsPressed
    {
        get
        {
            return _isPressed;
        }

        set
        {
            _isPressed = value;
        }
    }

    public bool IsHolding
    {
        get
        {
            return _isHolding;
        }

        set
        {
            _isHolding = value;
        }
    }

    public float CurrentTime
    {
        get
        {
            return _currentTime;
        }

        set
        {
            _currentTime = value;
        }
    }

    public float HoldingTime
    {
        get
        {
            return _holdingTime;
        }

        set
        {
            _holdingTime = value;
        }
    }



    // Update is called once per frame
    void Update () {


		if (IsPressed) {

			CurrentTime += Time.deltaTime;

            if (CurrentTime >= HoldingTime && !IsHolding) {
                IsHolding = true;

				Debug.LogError ("I am holding");
			}
		}
	}




	public void PointerUp()
	{

        if (IsHolding)
        {
            Debug.Log("Button was holding");

        }
        else
        { // single tap detected

            Debug.Log("Single tap detected");

        }


		IsPressed = false;
        IsHolding = false;

	}

	public void PointerDown()
	{
		IsPressed = true;
        HoldingTime =  1f;
		CurrentTime = 0;
	}

}
