using System;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    private IInteractable interactable;


    private void Awake()
    {
        interactable = GetComponent<IInteractable>();
    }

    public void Interact(GameObject gameObject)
    {
        interactable.Interact(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractor interactor = other.GetComponent<IInteractor>();

        if (interactor != null)
        {
            interactor.EnterInteractZone(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractor interactor = other.GetComponent<IInteractor>();

        if (interactor != null)
        {
            interactor.LeaveInteractZone(this);
        }
    }
}
