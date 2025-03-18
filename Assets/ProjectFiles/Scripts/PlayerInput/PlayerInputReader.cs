using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputReader : MonoBehaviour, IInputContainer
{
    private const string MOVE_ACTION_NAME = "Move";
    private const string JUMP_ACTION_NAME = "Jump";

    [SerializeField] private InputActionAsset inputActionAsset;

    #region Actions
    private InputAction _movementAction;
    private InputAction _jumpAction;
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
        _jumpAction     = inputActionAsset.FindAction(JUMP_ACTION_NAME, true);
    }

    private void SubscribeToActions()
    {
        _jumpAction.performed += JumpActionOnPerformed;
    }

    private void JumpActionOnPerformed(InputAction.CallbackContext obj)
    {
        NotifyInputAction(InputActionType.Jump);
    }
}
