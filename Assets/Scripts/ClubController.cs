using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubController : MonoBehaviour
{
    public Transform clubVisual;
    public ClubObject currentClub;
    public ClubObject[] clubs;

    Vector3 mousePos;

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
        preRotation = transform.rotation.eulerAngles;
        newRotation = preRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
            LookAtMouse();
       
        if (Input.GetKeyDown(KeyCode.E))
		{
            canHit = true;
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, clubVisual.localScale.y, rayLayerMask);
            if (hit.collider != null)
			{
                if (hit.collider.tag == "Ball")
				{
                    Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
                    if (rb.velocity.magnitude >= .25f)
					{
                        Debug.Log("Moving");
                        return;
					} else
					{
                        Debug.Log("Still");
                        Debug.Log(angularVelocity);
					}
                    rb.AddForce(((Quaternion.Euler(0f, 0f, 90f + currentClub.clubAngle) * rayDirection).normalized * (angularVelocity / forceDamp) * currentClub.clubPower), ForceMode2D.Impulse);
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

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, transform.right));
        Gizmos.DrawRay(new Ray(Vector3.zero + new Vector3(0f, 2f, 0f), Quaternion.Euler(0f, 0f, 90f) * rayDirection));
	}

}
