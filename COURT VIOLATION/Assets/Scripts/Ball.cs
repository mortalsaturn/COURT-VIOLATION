using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public string lastHitter;
    public Vector3 destination;
    public bool pauseOnCollision;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        FindDirection();
        DrawTrajectory(rb);
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
                float sx = hitboxScript.hitArea.localScale.x;
                float sz = hitboxScript.hitArea.localScale.z;
                float cx = hitboxScript.hitArea.transform.position.x;
                float cz = hitboxScript.hitArea.transform.position.x;
                float xMin = -sx + cx;
                float xMax = sx + cx;
                float zMin = -sz + cz;
                float zMax = sz + cz;

                hitboxScript.AdjustLaunchAngle(rb, hitbox.transform, xMin, xMax, zMin, zMax);
                hitboxScript.Launch(rb, hitbox.transform.forward, hitboxScript.launchVel);
                lastHitter = hitboxScript.user;
                Debug.Log("Predicted to land at: " + hitboxScript.PredictLandingPoint(rb));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        lastHitter = collision.gameObject.name;
        Debug.Log("Landed at: " + transform.position);
        if (pauseOnCollision)
        {
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }
    }

    void DrawTrajectory(Rigidbody rb, int steps = 30, float stepTime = 0.1f)
    {
        Vector3 pos = rb.position;
        Vector3 vel = rb.linearVelocity;
        Vector3 g = Physics.gravity;

        for (int i = 0; i < steps; i++)
        {
            Vector3 newPos = pos + vel * stepTime + 0.5f * g * stepTime * stepTime;
            Debug.DrawLine(pos, newPos, Color.yellow);

            vel += g * stepTime;
            pos = newPos;

            if (pos.y <= 0f)
                break;
        }
    }
}
