using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int _walk     = Animator.StringToHash("Walk");
    private static readonly int _run      = Animator.StringToHash("Run");
    private static readonly int _jump     = Animator.StringToHash("Jump");
    private static readonly int _interact = Animator.StringToHash("Interact");


    public void SetWalk(bool state)
    {
        animator.SetBool(_walk, state);
    }

    public void SetRun(bool state)
    {
        animator.SetBool(_run, state);
    }

    public void SetJump()
    {
        animator.SetTrigger(_jump);
    }

    public void SetInteract()
    {
        animator.SetTrigger(_interact);
    }
}
