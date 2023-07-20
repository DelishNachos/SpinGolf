using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallPhysics : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject cameraConfider;

    private bool isgrounded;
    public LayerMask layerMask;
    public float rayLength;

    public float groundDrag;
    public float airDrag;
    public float maxSpeed;

    public static Action<bool, Vector2> ballOffCamera;
    public static Action<Vector2> stopped;
    private bool isStopped = true;
    public bool canBeHit { get; private set; }
    private bool calledEvent;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
        cameraConfider = GameObject.FindGameObjectWithTag("CameraConfider");
	}

	void Update()
    {        
        if (rb.velocity.magnitude > maxSpeed)
		{
            rb.velocity = rb.velocity.normalized * maxSpeed;
		}

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, rayLength, layerMask);
        if (hit.collider != null)
		{
            isgrounded = true;
            rb.drag = groundDrag;
		} else
		{
            isgrounded = false;
            rb.drag = airDrag;
		}

        canBeHit = true;
        //canBeHit = rb.velocity.magnitude > 0.05f ? false : true;

        if (rb.velocity.magnitude < .1f && isgrounded && !DataHolder.isInWater && !DataHolder.isInHole)
		{
            if (!isStopped)
			{
                stopped?.Invoke(rb.transform.position);
                isStopped = true;
			}
		} else
		{
            isStopped = false;
		}

        if (cameraConfider == null)
            return;

        if (transform.position.y > cameraConfider.GetComponent<Collider2D>().bounds.max.y && !calledEvent)
		{
            ballOffCamera?.Invoke(true, new Vector2(transform.position.x, transform.position.y - cameraConfider.GetComponent<Collider2D>().bounds.max.y));
            
		} else
		{
            ballOffCamera?.Invoke(false, new Vector2(transform.position.x, transform.position.y));
		}
    }
}
