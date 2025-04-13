using System;
using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    private MeshCollider[] _meshCollider;
    private CapsuleCollider _capsuleCollider;


    private void Awake()
    {
        FindMeshColliders();
        FindCapsuleCollider();
    }

    public void SwitchToCapsule()
    {
        foreach (MeshCollider meshCollider in _meshCollider)
        {
            meshCollider.enabled = false;
        }

        if (_capsuleCollider)
        {
            _capsuleCollider.enabled = true;
        }
    }

    public void SwitchToMesh()
    {
        foreach (MeshCollider meshCollider in _meshCollider)
        {
            meshCollider.enabled = true;
        }

        if (_capsuleCollider)
        {
            _capsuleCollider.enabled = false;
        }
    }

    private void FindMeshColliders()
    {
        _meshCollider = GetComponentsInChildren<MeshCollider>();
    }

    private void FindCapsuleCollider()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (!_capsuleCollider)
        {
            _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        }
    }
}
