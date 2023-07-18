using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaterScript : MonoBehaviour
{
	public static Action StuckInWater;
	private bool isComplete;
	

	Rigidbody2D rb;
	SpriteRenderer SR;
	private float storedGravityScale;
	public Color ballInWaterColor;
	private Color storedBallColor;

	//private bool inWater;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			//inWater = true;
			DataHolder.isInWater = true;
			rb = collision.GetComponent<Rigidbody2D>();
			SR = collision.GetComponent<SpriteRenderer>();
			storedBallColor = SR.color;
			SR.color = ballInWaterColor;
			storedGravityScale = rb.gravityScale;
			rb.gravityScale = .2f;
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			//inWater = false;
			DataHolder.isInWater = false;
			rb.gravityScale = storedGravityScale;
			SR.color = storedBallColor;
		}
	}
}
