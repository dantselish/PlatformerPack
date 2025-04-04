using UnityEngine;

public class SwiperBouncer : MonoBehaviour
{
    private float _power;


    public void SetPower(float power)
    {
        _power = power;
    }

    private void OnCollisionEnter(Collision other)
    {
        Vector3 forceDirection = (other.transform.position - transform.position).normalized;
        forceDirection.y = 0;

        PlayerBasicController playerBasicController = other.gameObject.GetComponent<PlayerBasicController>();
        if (playerBasicController)
        {
            playerBasicController.KnockDown(forceDirection * _power);
        }
        else
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            if (rb)
            {
                rb.AddForce(forceDirection * _power, ForceMode.Impulse);
            }
        }
    }
}
