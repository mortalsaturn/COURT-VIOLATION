using UnityEngine;

public class Hitbox : MonoBehaviour
{
    float launchVelocity;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 150, Color.pink);
    }

    void Launch(Rigidbody rigidbody, Vector3 launchAngle, float launchVel)
    {
        rigidbody.linearVelocity = launchAngle.normalized * launchVel;
    }
}
