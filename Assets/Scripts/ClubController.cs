using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

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
    public List<ClubObject> clubs;
    public static Action<int> changedClub;

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

	private void OnEnable()
	{
        BallPhysics.stopped += MoveClub;
        DataHolder.addedClub += UpdateClubList;
	}

	private void OnDisable()
	{
        BallPhysics.stopped -= MoveClub;
        DataHolder.addedClub -= UpdateClubList;
    }

	// Start is called before the first frame update
	void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        Invoke("TurnOnHit", .2f);
        currentClubSR = clubVisual.gameObject.GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DataHolder.hasPutter && (clubs == null || clubs.Count == 0))
		{
            UpdateClubList(DataHolder.putter);
            currentClubIndex = 0;
            currentClub = clubs[currentClubIndex];
            currentClubSR.sprite = currentClub.clubVisual;
            //changedClub?.Invoke(currentClub.UIVisual, 0);
        }

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
                if (i < clubs.Count)
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
        if (changeIndex >= clubs.Count)
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
            changeIndex = clubs.Count - 1;
        }
        ChangeClubIndex(changeIndex);
    }

    private void ChangeClubIndex(int index)
	{
        if (currentClubIndex == index)
            return;

        currentClubIndex = index;
        DataHolder.currentClubIndex = currentClubIndex;
        currentClub = clubs[currentClubIndex];
        currentClubSR.sprite = currentClub.clubVisual;
        changedClub?.Invoke(index);
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

    private void MoveClub(Vector2 ballPos)
	{
        Debug.Log("Club Moved");
        canHit = false;
        TweenCallback tweenCallback = null;
        tweenCallback += ResetRotations;
        tweenCallback += TurnOnHit;
        transform.DOMove(new Vector3(ballPos.x - .3f, ballPos.y + 1.85f, 0f), 1f).OnComplete(tweenCallback);
	}

    private void ResetRotations()
	{
        preRotation = mousePos - transform.position;
        newRotation = preRotation;
        Debug.Log("Reset Rotations");
	}

    private void UpdateClubList(ClubObject club)
	{
        if (clubs == null)
		{
            clubs = new List<ClubObject>();
		}

        if (clubs.Contains(club))
            return;

        clubs.Add(club);
	}
}
