using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClubController : MonoBehaviour
{
    private KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0
    };

    public Transform clubVisual;
    private ClubObject currentClub;
    private SpriteRenderer currentClubSR;
    private int currentClubIndex;
    public ClubObject[] clubs;
    public static Action<Sprite, int> changedClub;

    Vector3 mousePos;
    private bool isRight;

	#region Collision Detection Vars
	Vector2 preRotation;
    Vector2 newRotation;
    float angle;
    float angleStep;
    public int rayAmount;
    private Vector3 rayDirection;
    public LayerMask rayLayerMask;
    private bool canMove = true;
    private bool canHit;
    [SerializeField] private float angularVelocity;
    public float forceDamp;
	#endregion

	// Start is called before the first frame update
	void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Invoke("TurnOnHit", .2f);
        currentClubSR = clubVisual.gameObject.GetComponent<SpriteRenderer>();
        currentClubIndex = 0;
        currentClub = clubs[currentClubIndex];
        currentClubSR.sprite = currentClub.clubVisual;
        changedClub?.Invoke(currentClub.UIVisual, currentClubIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
            LookAtMouse();

        if (Input.GetKeyDown(KeyCode.E)) {
            ChangeClubForward();
		}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeClubBackward();
        }

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                int numPressed = i + 1;
                if (i < clubs.Length)
				{
                    ChangeClubIndex(i);
				}
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
		{
            ChangeSide();
		}
    }

	private void FixedUpdate()
	{
        if (!canMove)
            return;

        if (!canHit)
            return;

        newRotation = mousePos - transform.position;
        angle = CalculateAngle();
        angleStep = angle / rayAmount;
        angularVelocity = angle / Time.fixedDeltaTime;
        for (int i = 0; i < rayAmount; i++)
		{
            rayDirection = Quaternion.AngleAxis(i * angleStep, transform.forward) * preRotation;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, 2f, rayLayerMask);
            if (hit.collider != null)
			{
                if (hit.collider.tag == "Ball")
				{
                    Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                    if (rb.velocity.magnitude >= .25f)
					{
                        return;
					}
                    rb.AddForce(Vector2.ClampMagnitude((Quaternion.Euler(0f, 0f, 90f + currentClub.clubAngle) * rayDirection).normalized * (angularVelocity / forceDamp), currentClub.clubMaxPower), ForceMode2D.Impulse);
                    Debug.Log(rb.velocity);
                    /*canMove = false;
                    float rotZ = Mathf.Atan2(rayDirection.y, rayDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);*/
                    return;
				}
			}
        }
        preRotation = newRotation;
	}

	void LookAtMouse()
	{
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + 90);
    }

    private float CalculateAngle()
	{
        return Vector2.SignedAngle(preRotation, newRotation);
    }

    private void TurnOnHit()
	{
        canHit = true;
	}

	/*private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.right));
        Gizmos.DrawRay(new Ray(Vector3.zero + new Vector3(0f, 2f, 0f), Quaternion.Euler(0f, 0f, 90f) * rayDirection));
	}*/

    private void ChangeClubForward()
	{
        int changeIndex = currentClubIndex + 1;
        if (changeIndex >= clubs.Length)
		{
            changeIndex = 0;
		}
        ChangeClubIndex(changeIndex);
    }

    private void ChangeClubBackward()
	{
        int changeIndex = currentClubIndex - 1;
        if (changeIndex < 0)
        {
            changeIndex = clubs.Length - 1;
        }
        ChangeClubIndex(changeIndex);
    }

    private void ChangeClubIndex(int index)
	{
        currentClubIndex = index;
        currentClub = clubs[currentClubIndex];
        currentClubSR.sprite = currentClub.clubVisual;
        changedClub?.Invoke(currentClub.UIVisual, index);
    }

    private void ChangeSide()
	{
        if (!isRight)
		{
            transform.position += new Vector3(.6f, 0f, 0f);
            currentClubSR.flipX = true;
            isRight = true;
		} else
		{
            transform.position -= new Vector3(.6f, 0f, 0f);
            currentClubSR.flipX = false;
            isRight = false;
        }
	}
}
