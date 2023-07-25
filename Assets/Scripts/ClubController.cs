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
        CameraControls.movingEvent += CanRotateEvent;
        CheckForEnd.levelComplete += TurnOffControls;
	}

	private void OnDisable()
	{
        BallPhysics.stopped -= MoveClub;
        DataHolder.addedClub -= UpdateClubList;
        CameraControls.movingEvent -= CanRotateEvent;
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

        if (Input.GetMouseButtonDown(0))
		{
            ResetRotations();
		}

        if (Input.GetMouseButton(0))
		{
            canHit = true;
		} else
		{
            canHit = false;
		}

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
        for (int i = 0; i < clubs.Count - 1; i++)
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
        //canHit = false;
        TweenCallback tweenCallback = null;
        //tweenCallback += ResetRotations;
        //tweenCallback += TurnOnHit;
        transform.DOMove(new Vector3(ballPos.x - .3f, ballPos.y + 1.85f, 0f), 1f).OnComplete(tweenCallback);
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

    private void TurnOffControls(int hits)
	{
        canHit = false;
        canMove = false;
	}
}
