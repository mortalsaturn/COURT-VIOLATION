using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rigidbody;
    public string lastHitter;

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
        Vector3 flatVelocity = new(velocity.x, 0f, velocity.z);
        if (flatVelocity.sqrMagnitude > 0.0001f)
        {
            Vector3 direction = flatVelocity.normalized;
            Debug.DrawRay(transform.position, direction * 150, Color.red);
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            GameObject hitbox = trigger.gameObject;
            Hitbox hitboxScript = trigger.gameObject.GetComponent<Hitbox>();

            if (lastHitter != hitboxScript.parent)
            {
                hitboxScript.Launch(
                    rigidbody,
                    hitbox.transform.forward,
                    hitboxScript.launchVelocity
                );
                lastHitter = hitboxScript.parent;
            }
        }
    }
}
