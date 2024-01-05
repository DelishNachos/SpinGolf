using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckForEnd : MonoBehaviour
{
    public static Action<int, float> levelComplete;
	private bool isComplete;

	private bool inHole;
	private float timeToComplete = 1f;
	private float currentTime;

	public AudioClip inHoleSound;

	public static Action<bool, GameObject> VisibleEvent;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			inHole = true;
			DataHolder.isInHole = true;
			SoundEffectsManager.Instance.PlayEffectAudio(inHoleSound);
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			inHole = false;
			DataHolder.isInHole = false;
			SoundEffectsManager.Instance.StopEffectAudio();
		}
	}

	private void Update()
	{
		if (inHole)
		{
			if (currentTime > 0f)
			{
				currentTime -= Time.deltaTime;
			} else
			{
				CallEvent();
			}
		} else
		{
			currentTime = timeToComplete;
		}

		Vector2 pos = Camera.main.WorldToViewportPoint(transform.position);
		/*if ((pos.x <= 1 || pos.x >= 0) && (pos.y <= 1 || pos.y >= 0))
		{
			VisibleEvent?.Invoke(true, this.gameObject);
			Debug.Log("Became Visible");
		} else
		{
			VisibleEvent?.Invoke(false, this.gameObject);
			Debug.Log("Became Invisible");
		}*/
	}

	public void OnBecameVisible()
	{
		
		
	}
	public void OnBecameInvisible()
	{
		
		
	}

	private void CallEvent()
	{
		if (!isComplete)
		{
			levelComplete?.Invoke(DataHolder.hits, DataHolder.currentTime);
			isComplete = true;
		}
	}
}
