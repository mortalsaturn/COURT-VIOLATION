using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float launchVelocity;
    public string parent;

    void Start()
    {
        parent = transform.parent.gameObject.name;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 150, Color.pink);
    }

    public void Launch(Rigidbody rigidbody, Vector3 launchAngle, float launchVel)
    {
        rigidbody.linearVelocity = launchAngle.normalized * launchVel;
    }
}
