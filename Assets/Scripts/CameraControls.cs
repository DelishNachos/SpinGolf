using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    public static Action<bool> movingEvent;

    [SerializeField] private float panSpeed;
    [SerializeField] private float returnSpeed;

    private bool isMoving;

    private CinemachineBrain brain;
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera moveCam;
    [SerializeField] private Collider2D confiner;
    private Transform camTrans;

    private PlayerInput playerInput;
    private InputAction moveCameraAction;
    private InputAction resetCameraAction;
    private InputAction mousePosition;

    private Vector3 origin;
    private Vector3 difference;
    private GameObject ballObject;

    private bool drag;

    private void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
        ballObject = FindObjectOfType<BallPhysics>().gameObject;

        playerInput = GetComponent<PlayerInput>();
        moveCameraAction = playerInput.actions["CameraDrag"];
        resetCameraAction = playerInput.actions["ResetCamera"];
        mousePosition = playerInput.actions["MousePosition"];
	}

	// Start is called before the first frame update
	void Start()
    {
        playerCam.Follow = ballObject.transform;
        moveCam.Priority = 0;
        camTrans = moveCam.VirtualCameraGameObject.transform;
    }

	private void OnEnable()
	{
        moveCameraAction.Enable();
        resetCameraAction.Enable();
        mousePosition.Enable();

        moveCameraAction.started += StartDrag;
        moveCameraAction.canceled += EndDrag;
        resetCameraAction.started += ResetDrag;
	}

	private void OnDisable()
	{
        moveCameraAction.Disable();
        resetCameraAction.Disable();
        mousePosition.Disable();

        moveCameraAction.started -= StartDrag;
        moveCameraAction.canceled -= EndDrag;
        resetCameraAction.started -= ResetDrag;
    }

	private void LateUpdate()
	{
        if (isMoving)
		{
            //playerCam.Follow = null;
            difference = Camera.main.ScreenToWorldPoint(mousePosition.ReadValue<Vector2>()) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(mousePosition.ReadValue<Vector2>());
            }
        }

		if (drag)
		{
            camTrans.position = origin - difference;
		}
	}

    private void StartDrag(InputAction.CallbackContext callbackContext)
	{
        isMoving = true;
        playerCam.Priority = 0;
        moveCam.Priority = 1;
    }

    private void EndDrag(InputAction.CallbackContext callbackContext)
	{
        isMoving = false;
        drag = false;
        
    }

    private void ResetDrag(InputAction.CallbackContext callbackContext)
	{
        playerCam.Priority = 1;
        moveCam.Priority = 0;
    }
}
