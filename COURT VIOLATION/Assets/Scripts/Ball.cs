using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rigidbody;
    public string lastHitter;
    public Vector3 destination;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        FindDirection();
    }

    public void FindDirection()
    {
        Vector3 velocity = rigidbody.linearVelocity;

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
                hitboxScript.Launch(
                    rigidbody,
                    hitbox.transform.forward,
                    hitboxScript.launchVelocity
                );
                lastHitter = hitboxScript.user;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        lastHitter = collision.gameObject.name;
    }
}
