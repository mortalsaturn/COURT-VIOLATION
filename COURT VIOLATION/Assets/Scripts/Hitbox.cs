using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float baseVel,
        launchVel,
        maxVel;
    public string user;

    void Start()
    {
        user = transform.root.gameObject.name;
    }

    void Update() { }

    public void Launch(Rigidbody rigidbody, Vector3 launchAngle, float launchVel)
    {
        rigidbody.linearVelocity = launchAngle.normalized * launchVel;
    }
}
