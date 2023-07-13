using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool isgrounded;
    public LayerMask layerMask;
    public float rayLength;

    public float groundDrag;
    public float airDrag;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
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
    }
}
