using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;


public class PlayerBasicController : MonoBehaviour, IInputObserver, IAnimatorCallbackReceiver, IInteractor, IPoppable, IRespawnable, ISquashable
{
    [Header("References")]
    [SerializeField] private Transform movementTransform;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private GameObject playerCameraPrefab;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private List<GroundDetector> groundDetectors;
    [SerializeField] private ParticleSystem popParticle;
    [SerializeField] private GameObject characterVisuals;

    [Header("Settings")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float rotationSpeed;

    [Space]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpTriggerResetTime;

    [Space]
    [SerializeField] private float squashTime;

    private Sequence _squashSequence;
    private InteractZone _currentInteractZone;
    private RespawnPoint _currentRespawnPoint;
    private InputValues _inputValues;
    private Camera _playerCamera;

    private bool _isGrounded;
    private bool _isUncontrolled;
    private bool _isPopped;
    private bool _isSquashed;
    private bool _isWaitingForJumpCallback;

    private float _jumpCallbackTimer;

    private Ray _squashCheckRay;

    private bool CanJump => _isGrounded && !_isUncontrolled && !_isPopped && !_isSquashed;
    private bool CanInteract => _isGrounded && !_isUncontrolled && !_isPopped && !_isSquashed && _currentInteractZone != null;
    private bool CanDance => _isGrounded && !_isUncontrolled && !_isPopped && !_isSquashed;
    private bool CanMove => !_isUncontrolled && !_isPopped;
    private bool CanLeaveUncontrolledState => _isGrounded && rb.linearVelocity.magnitude < 10f;


    private void Awake()
    {
        SpawnCamera();
        playerAnimator.AddCallbackReceiver(this);
        _isUncontrolled = false;
    }

    private void Start()
    {
        IInputContainer inputContainer = FindAnyObjectByType<PlayerInputReader>();
        inputContainer.Attach(this);
    }

    private void Update()
    {
        ProcessMovement();
        CheckIfGrounded();
        CountJumpTimer();
    }

    public void KnockDown(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
        rb.angularVelocity = Vector3.zero;
        SetUncontrolledState();
    }

    #region IPoppable
    public void Pop()
    {
        characterVisuals.SetActive(false);

        rb.useGravity = false;
        rb.isKinematic = true;

        popParticle.Play();

        _isPopped = true;
    }
    #endregion

    #region IRespawnable
    public bool TrySetAsNewRespawnPoint(RespawnPoint respawnPoint)
    {
        if (_currentRespawnPoint == respawnPoint)
        {
            return false;
        }
        else
        {
            _currentRespawnPoint = respawnPoint;
            return true;
        }
    }

    public void Respawn()
    {
        transform.position = _currentRespawnPoint.respawnTransform.position;
        transform.rotation = _currentRespawnPoint.respawnTransform.rotation;

        Unpop();
    }
    #endregion

    #region ISquashable
    public void Squash(Vector3 force, Vector3 contactPoint)
    {
        Vector3 direction = force.normalized;

        float colliderRadius = playerCollider.radius;
        float colliderHeight = (playerCollider.height * 0.5f) - colliderRadius;

        float dotProduct = Vector3.Dot(direction, transform.up);

        Vector3 oppositePoint;

        if (Mathf.Abs(dotProduct) > 0.5f)
        {
            float moveAmount = colliderHeight;
            oppositePoint = transform.position - (Mathf.Sign(dotProduct) * transform.up * moveAmount);
        }
        else
        {
            oppositePoint = playerCollider.ClosestPoint(contactPoint) - direction * colliderRadius;
        }

        _squashCheckRay = new Ray(oppositePoint, force.normalized);
        bool isPinned = Physics.Raycast(_squashCheckRay, out RaycastHit raycastHit, 0.5f);

        if (isPinned)
        {
            _isSquashed = true;
            PlaySquashSequence(raycastHit.normal);
        }
        else
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
    #endregion

    #region IInputObserver
    public void UpdateInputValues(InputValues inputValues)
    {
        _inputValues = inputValues;
    }

    public void UpdateAction(InputActionType inputActionType)
    {
        if (inputActionType == InputActionType.Jump)
        {
            if (_isPopped)
            {
                Respawn();
            }
            else if (CanJump)
            {
                playerAnimator.SetJump();
                StartJumpTimer();
            }
            else if (_isUncontrolled && CanLeaveUncontrolledState)
            {
                LeaveUncontrolledState();
            }
        }

        if (inputActionType == InputActionType.Interact)
        {
            TryInteract();
        }

        if (inputActionType == InputActionType.Dance)
        {
            TryDance();
        }
    }
    #endregion

    #region IAnimatorCallbackReceiver
    public void JumpUpCallback()
    {
        StopJumpTimer();
        Jump();
    }
    #endregion

    #region IInteractor
    public void EnterInteractZone(InteractZone interactZone)
    {
        _currentInteractZone = interactZone;
    }

    public void LeaveInteractZone(InteractZone interactZone)
    {
        if (_currentInteractZone == interactZone)
        {
            _currentInteractZone = null;
        }
    }
    #endregion

    private void ProcessMovement()
    {
        if (!CanMove)
        {
            playerAnimator.SetWalk(false);
            playerAnimator.SetRun(false);
            return;
        }

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

        if (_isSquashed)
        {
            translationVector *= 0.3f;
        }

        movementTransform.Translate(translationVector, Space.World);
        Quaternion lookRotation = Quaternion.LookRotation(translationVector);
        movementTransform.rotation = Quaternion.Slerp(movementTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void Jump(float additionForce = 0f)
    {
        rb.AddForce(Vector3.up * (jumpForce + additionForce), ForceMode.Impulse);
    }

    private void StartJumpTimer()
    {
        _isWaitingForJumpCallback = true;
    }

    private void StopJumpTimer()
    {
        _isWaitingForJumpCallback = false;
        _jumpCallbackTimer = 0;
    }

    private void CountJumpTimer()
    {
        if (!_isWaitingForJumpCallback)
        {
            return;
        }

        _jumpCallbackTimer += Time.deltaTime;
        if (_jumpCallbackTimer >= jumpTriggerResetTime)
        {
            playerAnimator.ResetJump();
            StopJumpTimer();
        }
    }

    private void CheckIfGrounded()
    {
        _isGrounded = false;
        foreach (GroundDetector groundDetector in groundDetectors)
        {
            if (groundDetector.HasCollision)
            {
                _isGrounded = true;
                break;
            }
        }

        playerAnimator.SetIsGrounded(_isGrounded);
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

    private void SetUncontrolledState()
    {
        _isUncontrolled = true;

        rb.constraints = RigidbodyConstraints.None;

        playerAnimator.SetIsControlled(false);
        playerAnimator.SetKnockDown();
    }

    private void LeaveUncontrolledState()
    {
        _isUncontrolled = false;

        playerAnimator.SetIsControlled(true);

        //Rotate to stand
        Vector3 rotationEuler = transform.rotation.eulerAngles;
        rotationEuler.x = 0;
        rotationEuler.z = 0;
        transform.rotation = Quaternion.Euler(rotationEuler);

        //Clean velocity
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Set constrains
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void TryInteract()
    {
        if (!CanInteract)
        {
            return;
        }

        playerAnimator.SetInteract();
        _currentInteractZone.Interact(gameObject);
    }

    private void TryDance()
    {
        if (!CanDance)
        {
            return;
        }

        playerAnimator.SetDance();
    }

    private void Unpop()
    {
        characterVisuals.SetActive(true);

        rb.isKinematic = false;
        rb.useGravity = true;

        _isPopped = false;
    }

    private void PlaySquashSequence(Vector3 direction)
    {
        direction = transform.InverseTransformDirection(direction);
        direction = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));

        if (_squashSequence != null && _squashSequence.IsPlaying())
        {
            _squashSequence.Kill(true);
        }

        Vector3 originalScale = characterVisuals.transform.localScale;
        Vector3 newScale = characterVisuals.transform.localScale;

        Debug.Log(direction.normalized);

        if (direction.x > direction.y && direction.x > direction.z)
        {
            newScale.x = 0.1f;
        }
        else if (direction.z > direction.x && direction.z > direction.y)
        {
            newScale.z = 0.1f;
        }
        else
        {
            newScale.y = 0.1f;
        }

        _squashSequence = DOTween.Sequence();
        _squashSequence.Append(characterVisuals.transform.DOScale(newScale, 0.1f));
        _squashSequence.AppendInterval(squashTime);
        _squashSequence.AppendCallback(() => _isSquashed = false);
        _squashSequence.Append(characterVisuals.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBounce));
    }

    private void OnDrawGizmos()
    {
        DrawSqushCheckRay();

        void DrawSqushCheckRay()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_squashCheckRay);
        }
    }
}
