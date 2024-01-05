using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ClubController : MonoBehaviour
{
    #region InputSystem
    private PlayerInput playerInput;
    private InputAction HitBallAction;
    private InputAction ChangeClubsAction;
    private InputAction MousePosition;
    private InputAction ChangeSideAction;
	#endregion

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
    private GameObject ball;

	#region Collision Detection Vars
	Vector2 preRotation;
    Vector2 newRotation;
    float angle;
    float angleStep;
    public int rayAmount;
    private Vector3 rayDirection;
    public LayerMask rayLayerMask;
    private bool canMove = true; //Can the club rotate
    private bool canHit;         //Can the club hit the ball
    private bool canSwing = true;//Can the club swing and hit the ball if can hit is true
    [SerializeField] private float angularVelocity;
    public float forceDamp;
    #endregion

    [SerializeField] private Color noHitColor;
    [SerializeField] private Color hitColor;
    private bool isComplete;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        HitBallAction = playerInput.actions["HitBall"];
        ChangeClubsAction = playerInput.actions["ScrollThroughClubs"];
        MousePosition = playerInput.actions["MousePosition"];
        ChangeSideAction = playerInput.actions["ChangeSides"];

        ball = FindObjectOfType<BallPhysics>().gameObject;
	}

	private void OnEnable()
	{
        HitBallAction.Enable();
        ChangeClubsAction.Enable();
        MousePosition.Enable();
        ChangeSideAction.Enable();

        HitBallAction.started += CanHitBall;
        HitBallAction.canceled += CantHitBall;
        ChangeClubsAction.started += ChangeClub;
        ChangeSideAction.started += ChangeSide;

        BallPhysics.stopped += MoveClub;
        DataHolder.addedClub += UpdateClubList;
        CheckForEnd.levelComplete += TurnOffControls;
	}

	private void OnDisable()
	{
        HitBallAction.Disable();
        ChangeClubsAction.Disable();
        MousePosition.Disable();
        ChangeSideAction.Disable();

        HitBallAction.started -= CanHitBall;
        HitBallAction.canceled -= CantHitBall; 
        ChangeClubsAction.started -= ChangeClub;
        ChangeSideAction.started -= ChangeSide;

        BallPhysics.stopped -= MoveClub;
        DataHolder.addedClub -= UpdateClubList;
        CheckForEnd.levelComplete -= TurnOffControls;
    }

	// Start is called before the first frame update
	void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        //Invoke("TurnOnHit", .2f);
        currentClubSR = clubVisual.gameObject.GetComponent<SpriteRenderer>();
        currentClubIndex = DataHolder.currentClubIndex;
        StartCoroutine(restartClubChange(currentClubIndex));

        transform.position = new Vector3(ball.transform.position.x - .3f, transform.position.y, transform.position.z);
        currentClubSR.flipX = false;
        isRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (DataHolder.hasPutter && (clubs == null || clubs.Count == 0))
		{
            UpdateClubList(DataHolder.putter);
            currentClubIndex = 0;
            currentClub = clubs[currentClubIndex];
            currentClubSR.sprite = currentClub.clubVisual;
            //changedClub?.Invoke(currentClub.UIVisual, 0);
        }*/
        if (DataHolder.isPaused)
            return;


        if (canMove)
            LookAtMouse();


        /*if (Input.GetKeyDown(KeyCode.E)) {
            ChangeClubForward();
		}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeClubBackward();
        }*/

        if (canHit)
		{
            if (!isComplete)
                currentClubSR.color = hitColor;
		} else
		{
            currentClubSR.color = noHitColor;
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
    }

	private void FixedUpdate()
	{
        if (DataHolder.isPaused)
            return;

        //Debug.Log("FixedUpdate1");
        if (!canMove)
            return;

        if (!canHit)
		{
            //preRotation = newRotation;
            //Debug.Log("!canhit");
            return;
        }

        //Debug.Log("FixedUpdate2");

        newRotation = mousePos - transform.position;
        angle = CalculateAngle();
        angleStep = angle / rayAmount;
        angularVelocity = angle / Time.fixedDeltaTime;
        /*if (angularVelocity < 1f)
            return;*/
        for (int i = 0; i < rayAmount; i++)
		{
            rayDirection = Quaternion.AngleAxis(i * angleStep, transform.forward) * preRotation;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, 2f, rayLayerMask);
            if (hit.collider != null)
			{
                if (hit.collider.tag == "Ball")
				{
                    BallPhysics BP = hit.collider.GetComponent<BallPhysics>();
                    if (!BP.canBeHit)
                        return;
                    Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                    if (rb.velocity.magnitude >= .25f)
					{
                        return;
					}
                    Vector2 force = Vector2.ClampMagnitude((Quaternion.Euler(0f, 0f, 90f + currentClub.clubAngle) * rayDirection).normalized * (angularVelocity / forceDamp), currentClub.clubMaxPower);
                    rb.AddForce(force, ForceMode2D.Impulse);
                    float forceMag = force.magnitude;
                    SoundEffectsManager.Instance.PlaySpecialEffectAudio(currentClub.clip, ExtensionMethods.Remap(forceMag, 0, 50f, 0, 1));
                    DataHolder.addHit();
                    canSwing = false;
                    return;
				}
			}
        }
        preRotation = newRotation;
    }

	void LookAtMouse()
	{
        mousePos = MousePosition.ReadValue<Vector2>();
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
        ResetRotations();
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
        /*if (changeIndex >= clubs.Count)
		{
            changeIndex = 0;
		}*/
        for (int i = 0; i <= clubs.Count - 1; i++) //clubs.count - 1 = 3
        {
            if (changeIndex >= clubs.Count)
            {
                changeIndex = 0;
            }

            if (clubs[changeIndex] == null)
            {
                changeIndex++;
            }
            else
            {
                break;
            }
        }
        if (changeIndex == currentClubIndex)
            return;
        ChangeClubIndex(changeIndex);
    }

    private void ChangeClubBackward()
	{
        int changeIndex = currentClubIndex - 1;
        for (int i = 0; i < clubs.Count - 1; i++)
        {
            if (changeIndex < 0)
            {
                changeIndex = clubs.Count - 1;
            }

            if (clubs[changeIndex] == null)
            {
                changeIndex--;
            }
            else
            {
                break;
            }
        }
        if (changeIndex == currentClubIndex)
            return;
        ChangeClubIndex(changeIndex);
    }

    private void ChangeClubIndex(int index)
	{
        if (clubs[index] == null)
            return;
        
        currentClubIndex = index;
        DataHolder.currentClubIndex = currentClubIndex;
        currentClub = clubs[currentClubIndex];
        currentClubSR.sprite = currentClub.clubVisual;
        changedClub?.Invoke(index);
    }

    private void ChangeSide(InputAction.CallbackContext callbackContext)
	{
        if (!canSwing)
            return;

        if (!isRight)
		{
            //transform.position = ball.transform.position + new Vector3(.3f, 0f, 0f);
            transform.position = new Vector3(ball.transform.position.x + .3f, transform.position.y, transform.position.z);
            currentClubSR.flipX = true;
            isRight = true;
		} else
		{
            //transform.position = ball.transform.position - new Vector3(.3f, 0f, 0f);
            transform.position = new Vector3(ball.transform.position.x - .3f, transform.position.y, transform.position.z);
            currentClubSR.flipX = false;
            isRight = false;
        }
	}

    private void MoveClub(Vector2 ballPos)
	{
        if (isComplete)
            return;
        //canHit = false;
        TweenCallback tweenCallback = null;
        //tweenCallback += ResetRotations;
        //tweenCallback += TurnOnHit;
        transform.DOMove(new Vector3(ballPos.x - .3f, ballPos.y + 1.85f, 0f), 1f).OnComplete(tweenCallback);
        canSwing = true;
	}

    private void ResetRotations()
	{
        preRotation = mousePos - transform.position;
        newRotation = preRotation;
	}

    private void UpdateClubList(ClubObject club)
	{
        if (clubs == null)
		{
            clubs = new List<ClubObject>();
		}

        if (club == null)
        {
            clubs.Add(null);
        } else
		{
            if (clubs.Count < 4)
            {
                clubs.Add(club);
            }
            else
            {
                clubs.RemoveAt(club.referenceIndex);
                clubs.Insert(club.referenceIndex, club);
			}
		}



	}

    private void CanRotateEvent(bool bl)
	{
        canMove = !bl;
        /*if (!bl)
		{
            Invoke("TurnOnHit", Time.fixedDeltaTime);
		} else
		{
            canHit = false;
		}*/
	}

    private IEnumerator restartClubChange(int currentClubIndex)
	{
        yield return new WaitForSeconds(.1f);
        ChangeClubIndex(currentClubIndex);
    }

    private void TurnOffControls(int hits, float time)
	{
        canHit = false;
        canMove = false;
        isComplete = true;
	}

    private void CanHitBall(InputAction.CallbackContext callbackContext)
	{
        ResetRotations();
        canHit = true;
	}

    private void CantHitBall(InputAction.CallbackContext callbackContext)
	{
        canHit = false;
	}

    private void ChangeClub(InputAction.CallbackContext callbackContext)
	{
        if (callbackContext.ReadValue<float>() > 0)
		{
            ChangeClubForward();
		} else
		{
            ChangeClubBackward();
		}
	}
}
