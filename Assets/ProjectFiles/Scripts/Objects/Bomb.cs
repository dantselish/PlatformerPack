using UnityEngine;
using UnityEngine.Serialization;

public class Bomb : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem ignitionParticle;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleCallbackListener explosionCallback;
    [SerializeField] private MeshRenderer bombRenderer;

    [Header("Settings")]
    [SerializeField] private float detonationTime;
    [SerializeField] private float detonationRadius;
    [SerializeField] private float detonationThrowForce;

    private bool _isIgnited;

    private float _timer;


    private void Awake()
    {
        Ignite();

        explosionCallback.ParticleStopped += ExplosionCallbackOnParticleStopped;
    }

    private void Update()
    {
        if (_isIgnited)
        {
            _timer += Time.deltaTime;

            if (_timer >= detonationTime)
            {
                Detonate();
            }
        }
    }

    public void Ignite()
    {
        _timer = 0;
        _isIgnited = true;
        ignitionParticle.Play();
    }

    private void Detonate()
    {
        PlayExplosionEffects();
        ThrowPlayerAndObjects();

        _isIgnited = false;
    }

    private void ThrowPlayerAndObjects()
    {
        Collider[] results = new Collider[10];
        int size = Physics.OverlapSphereNonAlloc(transform.position, detonationRadius, results);
        for (int i = 0; i < size; i++)
        {
            Collider resultCollider = results[i];

            Vector3 direction = (resultCollider.gameObject.transform.position - transform.position).normalized;

            PlayerBasicController playerBasicController = resultCollider.GetComponent<PlayerBasicController>();
            if (playerBasicController)
            {
                playerBasicController.KnockDown(direction * detonationThrowForce);
            }
            else
            {
                Rigidbody rb = resultCollider.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.AddForce(direction * detonationThrowForce, ForceMode.Impulse);
                }
            }
        }
    }

    private void PlayExplosionEffects()
    {
        ignitionParticle.Stop();
        explosionParticle.Play();
        bombRenderer.enabled = false;
    }

    private void ExplosionCallbackOnParticleStopped()
    {
        Destroy(gameObject);
    }
}
