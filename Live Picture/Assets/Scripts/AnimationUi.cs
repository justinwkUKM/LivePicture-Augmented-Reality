using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationUi
{

    bool run = false;

    Animation element_animation;
    AnimationState animation_state;
    readonly float frame_rate;
    AnimationEvent[] animation_events;

	private event Action process_end;

    float start_frame, end_frame;

    public AnimationUi(Animation element_animation)
    {

        if (element_animation == null)
            return;
		

		this.element_animation = element_animation;

        animation_state = element_animation[element_animation.clip.name];
		animation_state.speed = -1f;
		animation_state.time = 0;


		frame_rate = element_animation.clip.frameRate;
        animation_events = element_animation.clip.events;
        element_animation.Play();
        run = false;

    }



    public void update()
    {
        if (run)
        {
            if (start_frame < end_frame)
            {

                if (animation_state.time >= end_frame || !element_animation.isPlaying)
                {
                    animation_state.speed = 0f;
                    animation_state.time = end_frame;
					run = false;

                    if (process_end != null)
                    {
                        process_end();
                        process_end = null;
                    }
                }
            }
            else
            {
                if (animation_state.time <= end_frame || !element_animation.isPlaying)
                {
                    animation_state.speed = 0f;
                    animation_state.time = end_frame;
                    run = false;

                    if (process_end != null)
                    {
                        process_end();
                        process_end = null;
                    }
                }
            }
        }


    }


    public void Run(int start, int end, float speed)
    {

        if (run)
            return;
        else
            run = true;


        this.start_frame = ((float)start / frame_rate);
        this.end_frame = ((float)end / frame_rate);

        animation_state.speed = speed;
        animation_state.time = this.start_frame;
        element_animation.Play();
    }

	public IEnumerator Coroutine(int start, int end, float speed, Action action)
	{

		if (run)
			yield return null;
		else
			run = true;

		this.start_frame = ((float)start / frame_rate);
		this.end_frame = ((float)end / frame_rate);

		animation_state.speed = 0;
		animation_state.time = this.start_frame;
		element_animation.Play();

		yield return new WaitForEndOfFrame();

		animation_state.speed = speed;
		element_animation.Play();

		while (run)
		{
			if (start_frame < end_frame)
			{

				if (animation_state.time >= end_frame || !element_animation.isPlaying)
				{
					animation_state.speed = 0f;
					animation_state.time = end_frame;
					run = false;

					if (action != null)
					{
						action();
						action = null;
					}
				}
			}
			else
			{
				if (animation_state.time <= end_frame || !element_animation.isPlaying)
				{
					animation_state.speed = 0f;
					animation_state.time = end_frame;
					run = false;

					if (action != null)
					{
						action();
						action = null;
					}
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}


	public void Set(int frame)
	{
		run = false;


		this.start_frame = ((float)frame / frame_rate);
		this.end_frame = start_frame;

		animation_state.speed = 0;
		animation_state.time = this.start_frame;
		element_animation.Play();
	}

	private void setButtonEventEnd(Action process_event)
    {
        process_end = null;
        process_end += process_event;
    }


}
