using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    public static Action<bool> movingEvent;

    [SerializeField] private float panSpeed;
    [SerializeField] private float returnSpeed;

    private bool isMoving;
    private bool canMove = true;
    private bool wait;

    private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera moveCam;
    [SerializeField] private Collider2D confiner;
    private Transform camTrans;

    private Vector2 moveVector;

	private void Awake()
	{
        brain = GetComponent<CinemachineBrain>();		
	}

	// Start is called before the first frame update
	void Start()
    {
        playerCam.Priority = 1;
        moveCam.Priority = 0;
        camTrans = moveCam.VirtualCameraGameObject.transform;
    }

	private void OnEnable()
	{
        
	}

	private void OnDisable()
	{
        
    }

	// Update is called once per frame
	void Update()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");


        if ((Input.GetKeyDown(KeyCode.Space) && isMoving))
		{
            isMoving = false;
            canMove = false;
            wait = true;
            Invoke("ChangeWait", .1f);          
            playerCam.Priority = 1;
            moveCam.Priority = 0;
            //camTrans.position = Vector3.Lerp(camTrans.position, playerCam.VirtualCameraGameObject.transform.position * returnSpeed, Time.deltaTime);
            //camTrans.position = playerCam.VirtualCameraGameObject.transform.position;
        }

        if (playerCam.Priority == 1 && !wait)
		{
            bool _move = canMove;
            canMove = !brain.IsBlending;
            if (_move != canMove)
			{
                SendEvent(false);
                _move = canMove;
            }
		}

        if (moveVector != Vector2.zero)
		{
            if (!canMove)
                return;

            bool _move = isMoving;
            isMoving = true;
            if (_move != isMoving)
			{
                SendEvent(true);
                _move = isMoving;
			}
            playerCam.Priority = 0;
            moveCam.Priority = 1;

            Vector3 targetPos = camTrans.position + (Vector3)moveVector * panSpeed;

            float vertExtent = moveCam.m_Lens.OrthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            targetPos.x = Mathf.Clamp(targetPos.x, confiner.bounds.min.x + horzExtent, confiner.bounds.max.x - horzExtent);
            targetPos.y = Mathf.Clamp(targetPos.y, confiner.bounds.min.y + vertExtent, confiner.bounds.max.y - vertExtent);

            camTrans.position = Vector2.Lerp(camTrans.position, targetPos, Time.deltaTime);
            camTrans.position = new Vector3(camTrans.position.x, camTrans.position.y, -10);
		}
    }

    private void SendEvent(bool bl)
	{
        movingEvent?.Invoke(bl);
	}

    private void ChangeWait()
	{
        wait = false;
	}
}
