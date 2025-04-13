using System;
using UnityEngine;

public class SquashTrigger : MonoBehaviour
{
    [SerializeField] private SquashDirection squashDirection;

    public SquashDirection SquashDirection => squashDirection;


    public event Action<SquashDirection> triggered;

    private void OnTriggerEnter(Collider other)
    {
        TrySquash(other.gameObject);
    }

    private void TrySquash(GameObject otherGo)
    {
        Squash squash = otherGo.GetComponent<Squash>();

        if (!squash)
        {
            return;
        }

        triggered?.Invoke(squashDirection);
    }
}
