using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputReader : MonoBehaviour, IInputContainer
{
    private const string MOVE_ACTION_NAME     = "Move";
    private const string SPRINT_ACTION_NAME   = "Sprint";
    private const string JUMP_ACTION_NAME     = "Jump";
    private const string INTERACT_ACTION_NAME = "Interact";
    private const string DANCE_ACTION_NAME    = "Dance";

    [SerializeField] private InputActionAsset inputActionAsset;

    #region Actions
    private InputAction _movementAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _danceAction;
    #endregion

    private InputValues _inputValues;

    private List<IInputObserver> _inputObservers;


    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (_movementAction != null)
        {
            _inputValues.movementInput = _movementAction.ReadValue<Vector2>();
        }

        UpdateInputValues();
    }

    public void Attach(IInputObserver inputObserver)
    {
        if (_inputObservers.Contains(inputObserver))
        {
            Debug.LogWarning($"Trying to subscribe one {nameof(IInputObserver)} to {nameof(IInputContainer)} more than one time.");
            return;
        }

        _inputObservers.Add(inputObserver);
    }

    public void Detach(IInputObserver inputObserver)
    {
        if (_inputObservers.Contains(inputObserver))
        {
            Debug.LogWarning($"Trying to remove not subscribed {nameof(IInputObserver)} from {nameof(IInputContainer)}.");
            return;
        }

        _inputObservers.Remove(inputObserver);
    }

    public void UpdateInputValues()
    {
        foreach (IInputObserver inputObserver in _inputObservers)
        {
            inputObserver.UpdateInputValues(_inputValues);
        }
    }

    public void NotifyInputAction(InputActionType inputActionType)
    {
        foreach (IInputObserver inputObserver in _inputObservers)
        {
            inputObserver.UpdateAction(inputActionType);
        }
    }

    private void Init()
    {
        _inputObservers = new List<IInputObserver>();

        FindActions();
        SubscribeToActions();
    }

    private void FindActions()
    {
        _movementAction = inputActionAsset.FindAction(MOVE_ACTION_NAME, true);
        _sprintAction   = inputActionAsset.FindAction(SPRINT_ACTION_NAME, true);
        _jumpAction     = inputActionAsset.FindAction(JUMP_ACTION_NAME, true);
        _interactAction = inputActionAsset.FindAction(INTERACT_ACTION_NAME, true);
        _danceAction    = inputActionAsset.FindAction(DANCE_ACTION_NAME, true);
    }

    private void SubscribeToActions()
    {
        _jumpAction.performed += JumpActionOnPerformed;

        _sprintAction.performed += SprintActionOnPerformed;
        _sprintAction.canceled += SprintActionOncanceled;

        _interactAction.performed += InteractActionOnPerformed;

        _danceAction.performed += DanceActionOnPerformed;
    }

    private void InteractActionOnPerformed(InputAction.CallbackContext obj)
    {
        NotifyInputAction(InputActionType.Interact);
    }

    private void SprintActionOncanceled(InputAction.CallbackContext obj)
    {
        _inputValues.sprint = false;
    }

    private void SprintActionOnPerformed(InputAction.CallbackContext obj)
    {
        _inputValues.sprint = true;
    }

    private void JumpActionOnPerformed(InputAction.CallbackContext obj)
    {
        NotifyInputAction(InputActionType.Jump);
    }

    private void DanceActionOnPerformed(InputAction.CallbackContext obj)
    {
        NotifyInputAction(InputActionType.Dance);
    }
}
