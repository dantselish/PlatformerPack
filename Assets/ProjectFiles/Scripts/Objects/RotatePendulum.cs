using System;
using DG.Tweening;
using UnityEngine;

public class RotatePendulum : MonoBehaviour
{
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 endRotation;

    [Space]
    [Tooltip("Time to move from start to end rotation")]
    [SerializeField] private float time;

    [Space]
    [SerializeField] private Ease ease;

    [Space]
    [SerializeField] private bool setupMode;

    private Sequence _sequence;


    private void Update()
    {
        CheckIfSequenceKilled();
    }

    private void CheckIfSequenceKilled()
    {
        if (_sequence == null || !_sequence.IsPlaying())
        {
            StartNewSequence();
        }
    }

    private void StartNewSequence()
    {
        _sequence = DOTween.Sequence();
        _sequence.SetAutoKill(false);
        _sequence.Pause();

        transform.rotation = Quaternion.Euler(startRotation);

        _sequence.Append(transform.DORotate(endRotation, time).SetEase(ease));
        _sequence.Append(transform.DORotate(startRotation, time).SetEase(ease));

        if (!setupMode)
        {
            _sequence.SetLoops(-1);
        }

        _sequence.Play();
    }
}
