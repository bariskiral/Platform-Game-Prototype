using UnityEngine;
using System.Collections;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float leftPushRange;
    [SerializeField] private float rightPushRange;
    [SerializeField] private float velocityThreshold;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = velocityThreshold;
    }

    void Update()
    {
        Push();
    }

    public void Push()
    {
        if (transform.rotation.z > 0
            && transform.rotation.z < rightPushRange
            && (rb.angularVelocity > 0)
            && rb.angularVelocity < velocityThreshold)
        {
            rb.angularVelocity = velocityThreshold;
        }
        else if (transform.rotation.z < 0
            && transform.rotation.z > leftPushRange
            && (rb.angularVelocity < 0)
            && rb.angularVelocity > velocityThreshold * -1)
        {
            rb.angularVelocity = velocityThreshold * -1;
        }

    }
}
