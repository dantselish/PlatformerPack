using System;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] private Vector3 localTranslate;

    private Vector3 _globalTranslate;


    private void Awake()
    {
        _globalTranslate = transform.TransformVector(localTranslate);
    }

    private void OnTriggerStay(Collider other)
    {
        other.transform.Translate(transform.TransformVector(localTranslate), Space.World);
    }
}
