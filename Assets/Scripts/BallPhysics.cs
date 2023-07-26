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

    public static Action<bool, Vector2> ballOffCameraY;
    public static Action ballOffCameraX;
    private bool offCameraX;
    private float currentTime;
    private float timeToComplete = 1f;
    public static Action<Vector2> stopped;
    private bool isStopped = true;
    public bool canBeHit { get; private set; }
    private bool calledEvent;

    public AudioClip ballRollingSound;
    private float volume;

	private void Awake()
	{
        rb = GetComponent<Rigidbody2D>();
        cameraConfider = GameObject.FindGameObjectWithTag("CameraConfider");
	}

	void Update()
    {
        bool _isGrounded = isgrounded;

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
            ballOffCameraY?.Invoke(true, new Vector2(transform.position.x, transform.position.y - cameraConfider.GetComponent<Collider2D>().bounds.max.y));
            
		} else
		{
            ballOffCameraY?.Invoke(false, new Vector2(transform.position.x, transform.position.y));
		}

        if (transform.position.x > cameraConfider.GetComponent<Collider2D>().bounds.max.x || transform.position.x < cameraConfider.GetComponent<Collider2D>().bounds.min.x)
		{
            offCameraX = true;
		} else
		{
            offCameraX = false;
		}

        if (offCameraX)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                CallEvent();
            }
        }
        else
        {
            currentTime = timeToComplete;
        }

        /*if (isgrounded)
		{
            volume = rb.velocity.magnitude;
            volume = ExtensionMethods.Remap(volume, 0, 20, 0, 1);
            SoundEffectsManager.Instance.ChangeSpecialEffectsVolume(volume);
		}

        if (_isGrounded != isgrounded)
		{
            if (isgrounded)
			{
                if (rb.velocity.magnitude > .25f)
                {
                    SoundEffectsManager.Instance.LoopSpecialEffectAudio(ballRollingSound, volume);
                    Debug.Log("PlayedSound");
                }
                else
                {
                    SoundEffectsManager.Instance.StopSpecialEffectAudio();
                    Debug.Log("StoppedSound");
                }
                _isGrounded = isgrounded;
			} else
			{
                SoundEffectsManager.Instance.StopSpecialEffectAudio();
                Debug.Log("StoppedSound");
                _isGrounded = isgrounded;
            }
		}*/
    }

    private void CallEvent()
	{
        ballOffCameraX?.Invoke();
	}
}
