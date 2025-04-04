using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public bool HasCollision { get; private set; }


    private void FixedUpdate()
    {
        HasCollision = Physics.Raycast(transform.position, Vector3.down, 0.55f);
    }
}
