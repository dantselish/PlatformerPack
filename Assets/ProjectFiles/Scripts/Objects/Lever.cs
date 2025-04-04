using DG.Tweening;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform activeTransform;
    [SerializeField] private Transform levelTransform;

    [Header("Settings")]
    [SerializeField] private float tweenTime;

    private bool _isActivated;


    public void Interact(GameObject interactor)
    {
        _isActivated = !_isActivated;
        Transform endTransform = _isActivated ? activeTransform : baseTransform;
        levelTransform.DOMove(endTransform.position, tweenTime);
        levelTransform.DORotateQuaternion(endTransform.rotation, tweenTime);
    }
}
