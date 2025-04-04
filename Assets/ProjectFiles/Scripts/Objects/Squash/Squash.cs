using System;
using UnityEngine;

public class Squash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider squashCollider;

    [Header("Settings")]
    [SerializeField] private float force;
    [SerializeField] private float collisionCooldownTime;

    private Vector3 direction;

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

    private void OnCollisionEnter(Collision other)
    {
        ISquashable squashable = other.gameObject.GetComponent<ISquashable>();

        if (squashable != null)
        {
            ContactPoint contactPoint = other.contacts[0];
            Vector3 direction = (other.collider.ClosestPoint(contactPoint.point) - contactPoint.point).normalized;
            this.direction = direction;
            squashable.Squash(direction * force, contactPoint.point);
            DisableCollision();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + direction * 300);
    }
}
