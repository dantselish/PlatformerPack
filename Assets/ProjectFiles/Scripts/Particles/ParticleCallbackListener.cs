using System;
using UnityEngine;

public class ParticleCallbackListener : MonoBehaviour
{
    public event Action ParticleStopped;

    private void OnParticleSystemStopped()
    {
        ParticleStopped?.Invoke();
    }
}
