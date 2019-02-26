using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BannerImageScript : MonoBehaviour {

	public Sprite[] imagesToSwap;
	public float swapTime = 3f;
	Image image;
	bool canChange = true;
	public static BannerImageScript bannerRef;

	// Use this for initialization
	void Awake () {
		image = GetComponent<Image>();
		image.sprite = imagesToSwap [0];
	}


	public void Swap(){
		int i = Random.Range (0, imagesToSwap.Length);
		image.sprite = imagesToSwap [i];

	}
}
