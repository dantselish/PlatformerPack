using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpringPad : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float squashDuration;
    [SerializeField] private float squashValue;
    [SerializeField] private float unsquashDuration;
    [SerializeField] private float cooldown;
    [SerializeField] private Vector3 force;

    private Sequence _bounceSequence;

    private Dictionary<GameObject, float> _cooldownForObject;


    private void Awake()
    {
        _cooldownForObject = new Dictionary<GameObject, float>();

        CreateSequence();
    }

    private void Update()
    {
        UpdateCoolDowns(Time.deltaTime);
    }

    private void UpdateCoolDowns(float deltaTime)
    {
        foreach (GameObject cooldownGo in _cooldownForObject.Keys)
        {
            _cooldownForObject[cooldownGo] -= deltaTime;
        }
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

    private void TryLaunch(GameObject objectToLaunch)
    {
        if (_cooldownForObject.ContainsKey(objectToLaunch) && _cooldownForObject[objectToLaunch] > 0f)
        {
            return;
        }

        Rigidbody rb = objectToLaunch.GetComponent<Rigidbody>();

        if (!rb)
        {
            return;
        }

        rb.AddForce(force, ForceMode.VelocityChange);
        PlayBounceEffect();
        _cooldownForObject[objectToLaunch] = cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryLaunch(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        TryLaunch(other.gameObject);
    }
}
