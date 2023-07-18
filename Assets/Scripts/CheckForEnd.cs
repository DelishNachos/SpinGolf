using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckForEnd : MonoBehaviour
{
    public static Action levelComplete;
	private bool isComplete;

	private bool inHole;
	private float timeToComplete = 1f;
	private float currentTime;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			inHole = true;
			DataHolder.isInHole = true;
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			inHole = false;
			DataHolder.isInHole = false;
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
	}

	private void CallEvent()
	{
		if (!isComplete)
		{
			levelComplete?.Invoke();
			isComplete = true;
		}
	}
}
