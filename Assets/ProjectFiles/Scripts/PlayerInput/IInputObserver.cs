public interface IInputObserver
{
    void UpdateInputValues(InputValues inputValues);

    void UpdateAction(InputActionType inputActionType);
}
