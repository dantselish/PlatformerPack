using System;
using UnityEngine;


public class SquashTriggersHolder : MonoBehaviour
{
    private SquashTrigger[] _squashTriggers;

    private event Action<SquashDirection> triggered;


    private void Awake()
    {
        FindSquashTriggers();
        SubscribeToTriggers();
    }

    public void SubscribeSqusahble(ISquashable squashable)
    {
        triggered += squashable.Squash;
    }

    private void FindSquashTriggers()
    {
        _squashTriggers = GetComponentsInChildren<SquashTrigger>();
    }

    private void SubscribeToTriggers()
    {
        foreach (SquashTrigger squashTrigger in _squashTriggers)
        {
            squashTrigger.triggered += OnSquashTriggered;
        }
    }

    private void OnSquashTriggered(SquashDirection squashDirection)
    {
        triggered?.Invoke(squashDirection);
    }
}
