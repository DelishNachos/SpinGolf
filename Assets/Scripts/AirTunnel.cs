using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTunnel : MonoBehaviour
{
	[SerializeField] private bool isUp;
	[SerializeField] private float maxSpeed;

	private Rigidbody2D rb;
	private BallPhysics BP;
	private float windForce = 50f;

	private float storedGroundDrag;
	private float storedAirDrag;
	private Vector2 storedVeloctity;
	private float storedMaxSpeed;

	private void Start()
	{
		if (!isUp)
		{
			windForce = -Mathf.Abs(windForce);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			Debug.Log("BallFound");
			rb = collision.GetComponent<Rigidbody2D>();
			BP = collision.GetComponent<BallPhysics>();
			
			storedAirDrag = BP.airDrag;
			storedGroundDrag = BP.groundDrag;
			storedVeloctity = rb.velocity;
			storedMaxSpeed = BP.maxSpeed;
			BP.airDrag = 0;
			BP.groundDrag = 0;
			//BP.maxSpeed = maxSpeed;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			Debug.Log("ForceAdded");
			rb.AddForce(Vector2.up * windForce, ForceMode2D.Force);
			rb.AddForce(storedVeloctity, ForceMode2D.Force);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Ball")
		{
			BP.airDrag = storedAirDrag;
			BP.groundDrag = storedGroundDrag;
			//BP.maxSpeed = storedMaxSpeed;
		}
	}
}
