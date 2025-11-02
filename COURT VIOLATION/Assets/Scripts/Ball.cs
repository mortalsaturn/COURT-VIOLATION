using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public string lastHitter;
    public Vector3 destination;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        FindDirection();
    }

    public void FindDirection()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 direction = velocity.normalized;

        Ray trajectoryRay = new(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(trajectoryRay, out hit))
        {
            destination = hit.point;
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            GameObject hitbox = trigger.gameObject;
            Hitbox hitboxScript = trigger.gameObject.GetComponent<Hitbox>();

            if (lastHitter != hitboxScript.user)
            {
                hitboxScript.Launch(rb, hitbox.transform.forward, hitboxScript.launchVel);
                lastHitter = hitboxScript.user;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        lastHitter = collision.gameObject.name;
    }
}
