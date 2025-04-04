using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private ParticleSystem confettiParticle;

    public Transform respawnTransform => respawnPoint.transform;


    private void OnTriggerEnter(Collider other)
    {
        IRespawnable respawnable = other.GetComponent<IRespawnable>();

        bool setAsNewRespawnPoint = false;

        if (respawnable != null)
        {
            setAsNewRespawnPoint = respawnable.TrySetAsNewRespawnPoint(this);
        }

        if (setAsNewRespawnPoint && confettiParticle)
        {
            confettiParticle.Play();
        }
    }
}
