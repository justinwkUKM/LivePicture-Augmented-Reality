using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectAnimator : MonoBehaviour
{


	public RectTransform rtCanvas;

	[Range(0,1)]
	public float relative;


	public float padding = 0.1f;

	private RectTransform rect_transform;

	private Vector2
		start_anchored_position,
		start_size_delta,

		end_anchored_position,
		end_size_delta;

	public Sprite[] logos;
	private Image image;

    void Start()
    {

        relative = 0;
        rect_transform = (RectTransform)transform;

        start_anchored_position = rect_transform.anchoredPosition;
        start_size_delta = rect_transform.sizeDelta;

        image = transform.GetComponent<Image>();

        float canvas_w = rtCanvas.rect.width;
        float canvas_h = rtCanvas.rect.height;





        float rect_w = canvas_w - canvas_w * padding * 2f;
        float rect_h = rect_w * rect_transform.sizeDelta.y / rect_transform.sizeDelta.x;


        Debug.Log("canvas_h[" + canvas_h + "] h[" + rect_h + "] rect_transform.sizeDelta[" + rect_transform.sizeDelta.ToString() + "]");

        float x = canvas_w * padding;
        float y = -(canvas_h / 2f);

        Debug.Log("start_anchored_position[" + start_anchored_position.ToString() + "] end_anchored_position[" + end_anchored_position.ToString() + "]");

        end_anchored_position = new Vector2(x, y);
        end_size_delta = new Vector2(rect_w, rect_h);
        Position(1f);

    }
	

	public void SetSprite(bool color)
	{
		if(color)
		{
			image.sprite = logos[0];
		}
		else
		{
			image.sprite = logos[1];
		}

	}

	public void Position(float relative)
	{

		Vector2 delta_pos = (end_anchored_position - start_anchored_position) * relative;
		Vector2 delta_size = (end_size_delta - start_size_delta) * relative;

		//Debug.Log("delta_pos[" + delta_pos.ToString() + "]");
		rect_transform.anchoredPosition = (start_anchored_position + delta_pos);
		rect_transform.sizeDelta = (start_size_delta + delta_size);

	}



	/*
	private void Update()
	{
		Position(relative);

	}*/
}
