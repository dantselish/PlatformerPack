using System;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject button;
    [SerializeField] private Transform basicPosition;
    [SerializeField] private Transform endPosition;

    [Header("Settings")]
    [SerializeField] private float pressTime;

    private float _timer;

    private bool _isTimerStarted;
    private bool _isReturn;

    public event Action Pressed;


    private void Awake()
    {
        Pressed += ButtonPressedDebug;
    }

    private void Update()
    {
        if (_isTimerStarted)
        {
            _timer += Time.deltaTime;
        }

        if (_isReturn)
        {
            _timer -= Time.deltaTime;
        }

        float deltaTime = Mathf.Clamp(_timer / pressTime, 0, 1);
        button.transform.position = Vector3.Lerp(basicPosition.position, endPosition.position, deltaTime);

        if (_timer >= pressTime && _isTimerStarted)
        {
            Pressed?.Invoke();
            StopTimer();
        }

        if (_timer <= 0)
        {
            _isReturn = false;
            StopTimer();
        }
    }

    private void StartTimer()
    {
        _isTimerStarted = true;
        _isReturn = false;
    }

    private void StopTimer()
    {
        _isTimerStarted = false;
    }

    private void ReturnToBasic()
    {
        StopTimer();
        _isReturn = true;
        _timer = pressTime - 0.01f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ReturnToBasic();
        }
    }

    private void ButtonPressedDebug()
    {
        Debug.Log($"Button {gameObject.name} at position {transform.position} was pressed");
    }
}
