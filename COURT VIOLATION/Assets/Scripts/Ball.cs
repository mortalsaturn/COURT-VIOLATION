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
    public Vector3 predictedPoint;
    public Vector3 destination;
    public bool pauseOnCollision;
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
                10f,
                destinationMask,
                QueryTriggerInteraction.Collide
            )
        )
        {
            destination = hit.point;

            // Check if the hit is the enemy's hit range
            isApproachingEnemy =
                hit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHitRange");
        }
        else
        {
            // No collision found
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
                if (hitboxScript.dynamicHitbox)
                {
                    float sx = hitboxScript.hitArea.localScale.x / 2;
                    float sz = hitboxScript.hitArea.localScale.z / 2;
                    float cx = hitboxScript.hitArea.transform.position.x;
                    float cz = hitboxScript.hitArea.transform.position.z;
                    float xMin = -sx + cx;
                    float xMax = sx + cx;
                    float zMin = -sz + cz;
                    float zMax = sz + cz;

                    hitboxScript.AdjustLaunchAngle(rb, hitbox.transform, xMin, xMax, zMin, zMax);
                }
                hitboxScript.Launch(rb, hitbox.transform.forward, hitboxScript.launchVel);
                lastHitter = hitboxScript.user;
                predictedPoint = hitboxScript.PredictLandingPoint(rb);
                Debug.Log("Predicted to land at: " + predictedPoint);
            }
        }
    }

    private void OnTriggerStay(Collider trigger)
    {
        inEnemyZone = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone");
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone"))
        {
            inEnemyZone = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
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
