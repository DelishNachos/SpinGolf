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

    public static Action<bool, float> ballOffCamera;
    private bool calledEvent;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
        cameraConfider = GameObject.FindGameObjectWithTag("CameraConfider");
	}

	void Update()
    {        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, rayLength, layerMask);
        if (hit.collider != null)
		{
            rb.drag = groundDrag;
		} else
		{
            rb.drag = airDrag;
		}

        if (cameraConfider == null)
            return;

        if (transform.position.y > cameraConfider.GetComponent<Collider2D>().bounds.max.y && !calledEvent)
		{
            ballOffCamera?.Invoke(true, transform.position.y - cameraConfider.GetComponent<Collider2D>().bounds.max.y);
            
		} else
		{
            ballOffCamera?.Invoke(false, transform.position.y);
		}
    }
}
