using System;
using UnityEngine;

public class Squash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider squashCollider;

    [Header("Settings")]
    [SerializeField] private float force;
    [SerializeField] private float collisionCooldownTime;

    private bool _isOnCooldown;
    private float _cooldownTimer;


    private void Update()
    {
        if (_isOnCooldown)
        {
            _cooldownTimer += Time.deltaTime;

            if (_cooldownTimer >= collisionCooldownTime)
            {
                EnableCollision();
            }
        }
    }

    private void DisableCollision()
    {
        squashCollider.enabled = false;
        _cooldownTimer = 0;
        _isOnCooldown = true;
    }

    private void EnableCollision()
    {
        _isOnCooldown = false;
        squashCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        SquashTrigger squashTrigger = other.GetComponent<SquashTrigger>();

        if (squashTrigger)
        {
            DisableCollision();
        }
    }
}
