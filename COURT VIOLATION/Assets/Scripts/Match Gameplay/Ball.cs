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
    float bounces = 0f;
    bool didBounce = false;
    public Transform spawn;
    public string lastHitter;
    public Vector3 predictedPoint;
    public Vector3 destination;
    public LayerMask destinationMask;

    [Header("Enemy")]
    public bool inEnemyZone;
    public bool inEnemyHitRange;
    public bool inEnemyWalkRange;
    public bool isApproachingEnemy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        FindDirection();
        DrawTrajectory(rb);
        if (bounces >= 2f)
        {
            bounces = 0;
            didBounce = false;
            ResetBall();
        }
    }

    void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        lastHitter = "";
        transform.position = spawn.position;
    }

    public void FindDirection()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 direction = velocity.normalized;

        Ray trajectoryRay = new(transform.position, direction);
        RaycastHit hit;

        if (
            Physics.Raycast(
                trajectoryRay,
                out hit,
                15f,
                destinationMask,
                QueryTriggerInteraction.Collide
            )
        )
        {
            destination = hit.point;
            Debug.DrawRay(transform.position, direction * 15, Color.red);

            isApproachingEnemy =
                hit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHitRange");
        }
        else
        {
            isApproachingEnemy = false;
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
                hitboxScript.AdjustLaunchAngle(
                    rb,
                    hitbox.transform,
                    hitboxScript.SetActiveHitArea()
                );

                hitboxScript.Launch(rb, hitbox.transform.forward, hitboxScript.launchVel);
                lastHitter = hitboxScript.user;
                predictedPoint = hitboxScript.PredictLandingPoint(rb);
                // Debug.Log("Predicted to land at: " + predictedPoint);
            }
        }
    }

    private void OnTriggerStay(Collider trigger)
    {
        inEnemyZone = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone");
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
        {
            bounces = 0f;
            didBounce = false;
        }

        if (trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone"))
        {
            inEnemyZone = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!(collision.gameObject.layer == LayerMask.NameToLayer("Ceiling")) && !didBounce)
        {
            bounces++;
            didBounce = true;
        }
        // Debug.Log("Bounces: " + bounces);
    }

    void OnCollisionExit(Collision collision)
    {
        didBounce = false;
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
