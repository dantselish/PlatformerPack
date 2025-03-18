using UnityEngine;


public class PlayerBasicController : MonoBehaviour, IInputObserver
{
    [Header("References")]
    [SerializeField] private Transform movementTransform;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Settings")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    private InputValues _inputValues;


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

        Vector3 translationVector = Vector3.zero;
        translationVector += Vector3.forward * inputVector.y;
        translationVector += Vector3.right * inputVector.x;

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

        movementTransform.Translate(translationVector);
    }
}
