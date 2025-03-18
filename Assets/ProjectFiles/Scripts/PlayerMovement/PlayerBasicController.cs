using UnityEngine;


public class PlayerBasicController : MonoBehaviour, IInputObserver
{
    [Header("References")]
    [SerializeField] private Transform movementTransform;
    [SerializeField] private PlayerAnimator playerAnimator;

    [Header("Settings")]
    [SerializeField] private float walkingSpeed;

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
        playerAnimator.SetJump();
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
        translationVector *= walkingSpeed * Time.deltaTime;
        movementTransform.Translate(translationVector);
        playerAnimator.SetWalk(true);
    }
}
