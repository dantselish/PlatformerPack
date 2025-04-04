using System;
using DG.Tweening;
using UnityEngine;

public class SpringPad : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float squashDuration;
    [SerializeField] private float squashValue;
    [SerializeField] private float unsquashDuration;
    [SerializeField] private Vector3 force;

    private Sequence _bounceSequence;


    private void Awake()
    {
        CreateSequence();
    }

    private void PlayBounceEffect()
    {
        if (_bounceSequence.IsComplete() || _bounceSequence.IsPlaying())
        {
            _bounceSequence.Restart();
        }

        _bounceSequence.Play();
    }

    private void CreateSequence()
    {
        float startYScale = transform.localScale.y;

        _bounceSequence = DOTween.Sequence();
        _bounceSequence.SetAutoKill(false);
        _bounceSequence.Pause();
        _bounceSequence.Append(transform.DOScaleY(squashValue, squashDuration).SetEase(Ease.OutExpo));
        _bounceSequence.Append(transform.DOScaleY(startYScale, unsquashDuration).SetEase(Ease.OutElastic));
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb)
        {
            rb.AddForce(force, ForceMode.Impulse);
            PlayBounceEffect();
        }
    }
}
