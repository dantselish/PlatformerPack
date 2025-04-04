using System;
using UnityEngine;

public class RotatingSwiper : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float bouncersPower;


    private void Awake()
    {
        SwiperBouncer[] bouncers = GetComponentsInChildren<SwiperBouncer>();
        foreach (SwiperBouncer swiperBouncer in bouncers)
        {
            swiperBouncer.SetPower(bouncersPower);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
