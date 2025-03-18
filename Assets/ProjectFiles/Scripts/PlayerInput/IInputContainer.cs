public interface IInputContainer
{
    void Attach(IInputObserver inputObserver);

    void Detach(IInputObserver inputObserver);

    void UpdateInputValues();

    void NotifyInputAction(InputActionType inputActionType);
}
