using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IPoppable poppable = other.GetComponent<IPoppable>();

        if (poppable != null)
        {
            poppable.Pop();
        }
    }
}
