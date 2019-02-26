using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour {




    public   GameObject ARCamera;
    public Transform target;



    float x, y, z;
    Vector3 pos;
    Vector3 cam;



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        pos = ARCamera.transform.position;

        x = x + ((pos.x - x) / 8);
        y = y + ((pos.y - y) / 8);
        z = z + ((pos.z - z) / 8);

        cam = transform.position;
        cam.x = x;
        cam.y = y;
        cam.z = z;

        transform.position = cam;


        transform.LookAt(target);
        cam = transform.eulerAngles;
        pos = ARCamera.transform.eulerAngles;
        cam.z = pos.z;
        transform.eulerAngles = cam; 


	}
}
