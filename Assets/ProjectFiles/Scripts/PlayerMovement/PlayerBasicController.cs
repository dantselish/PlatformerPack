using Unity.Cinemachine;
using UnityEngine;


public class PlayerBasicController : MonoBehaviour, IInputObserver
{
    [Header("References")]
    [SerializeField] private Transform movementTransform;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private GameObject playerCameraPrefab;

    [Header("Settings")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float rotationSpeed;

    private InputValues _inputValues;
    private Camera _playerCamera;


    private void Awake()
    {
        SpawnCamera();
    }

    private void Start()
    {
        IInputContainer inputContainer = FindAnyObjectByType<PlayerInputReader>();
        inputContainer.Attach(this);
    }

    private void Update()
    {
        ProcessMovement();
    }

    public void UpdateInputValues(InputValues inputValues)
    {
        _inputValues = inputValues;
    }

    public void UpdateAction(InputActionType inputActionType)
    {
        if (inputActionType == InputActionType.Jump)
        {
            playerAnimator.SetJump();
        }

        if (inputActionType == InputActionType.Interact)
        {
            playerAnimator.SetInteract();
        }
    }

    private void ProcessMovement()
    {
        Vector2 inputVector = _inputValues.movementInput;
        if (inputVector.Equals(Vector2.zero))
        {
            playerAnimator.SetWalk(false);
            playerAnimator.SetRun(false);
            return;
        }

        Vector3 cameraForward = _playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 translationVector = Vector3.zero;
        translationVector += cameraForward * inputVector.y;
        translationVector += cameraRight * inputVector.x;

        if (_inputValues.sprint)
        {
            playerAnimator.SetWalk(false);
            playerAnimator.SetRun(true);
            translationVector *= runningSpeed * Time.deltaTime;
        }
        else
        {
            playerAnimator.SetWalk(true);
            playerAnimator.SetRun(false);
            translationVector *= walkingSpeed * Time.deltaTime;
        }

        movementTransform.Translate(translationVector, Space.World);
        Quaternion lookRotation = Quaternion.LookRotation(translationVector);
        movementTransform.rotation = Quaternion.Slerp(movementTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void SpawnCamera()
    {
        GameObject cameraGO = Instantiate(playerCameraPrefab);
        _playerCamera = cameraGO.GetComponentInChildren<Camera>();

        if (!_playerCamera)
        {
            Debug.LogError($"No {nameof(Camera)} found!");
            return;
        }

        CinemachineCamera cinemachineCamera = cameraGO.GetComponentInChildren<CinemachineCamera>();

        if (!cinemachineCamera)
        {
            Debug.LogError($"No {nameof(CinemachineCamera)} found!");
            return;
        }

        cinemachineCamera.Target.TrackingTarget = cameraTarget;
    }
}
