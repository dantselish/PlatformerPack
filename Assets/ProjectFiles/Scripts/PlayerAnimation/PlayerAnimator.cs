using System;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int _walk          = Animator.StringToHash("Walk");
    private static readonly int _run           = Animator.StringToHash("Run");
    private static readonly int _jump          = Animator.StringToHash("Jump");
    private static readonly int _interact      = Animator.StringToHash("Interact");
    private static readonly int _is_grounded   = Animator.StringToHash("IsGrounded");
    private static readonly int _is_controlled = Animator.StringToHash("IsControlled");

    private event Action jumpUpEvent;


    public void AddCallbackReceiver(IAnimatorCallbackReceiver animatorCallbackReceiver)
    {
        jumpUpEvent += animatorCallbackReceiver.JumpUpCallback;
    }

    public void SetWalk(bool state)
    {
        animator.SetBool(_walk, state);
    }

    public void SetRun(bool state)
    {
        animator.SetBool(_run, state);
    }

    public void SetIsGrounded(bool state)
    {
        animator.SetBool(_is_grounded, state);
    }

    public void SetIsControlled(bool state)
    {
        animator.SetBool(_is_controlled, state);
    }

    public void SetJump()
    {
        animator.SetTrigger(_jump);
    }

    public void ResetJump()
    {
        animator.ResetTrigger(_jump);
    }

    public void SetInteract()
    {
        animator.SetTrigger(_interact);
    }

    public void SetDance()
    {
        animator.Play("Dance");
    }

    public void SetKnockDown()
    {
        animator.Play("Knock");
    }

    //Triggered from animation event
    public void TriggerJumpUpAnimationEvent()
    {
        jumpUpEvent?.Invoke();
    }
}
