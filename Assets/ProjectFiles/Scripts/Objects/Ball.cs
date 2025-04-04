using UnityEngine;


public class Ball : MonoBehaviour, IPoppable
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private float bounceForce;
    [SerializeField] private float verticalForce;


    public void Pop()
    {
        Debug.LogWarning("Ball pop logic not implemented!");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 kickDirection = (other.transform.position - other.gameObject.transform.position).normalized * bounceForce;
            kickDirection.y = verticalForce;

            rb.AddForce(kickDirection, ForceMode.Impulse);
        }
    }
}
