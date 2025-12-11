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
        if (inEnemyZone)
        {
            Debug.Log("In enemy zone");
        }
    }

    public void FindDirection()
    {
        Vector3 flatVel = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 direction = flatVel.normalized;

        Vector3 bodyLevel = new(transform.position.x, 1, transform.position.z);
        Ray trajectoryRay = new(bodyLevel, direction);
        RaycastHit hit;

        if (
            Physics.Raycast(
                trajectoryRay,
                out hit,
                Mathf.Infinity,
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
        Debug.DrawRay(bodyLevel, direction * 100);
    }

    void OnTriggerEnter(Collider trigger)
    {
        inEnemyZone = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone");
        inEnemyHitRange = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyHitRange");
        inEnemyWalkRange = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyWalkRange");

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
                Debug.Log("Predicted to land at: " + hitboxScript.PredictLandingPoint(rb));
            }
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone"))
        {
            inEnemyZone = false;
        }
        if (trigger.gameObject.layer == LayerMask.NameToLayer("EnemyHitRange"))
        {
            inEnemyHitRange = false;
        }
        if (trigger.gameObject.layer == LayerMask.NameToLayer("EnemyWalkRange"))
        {
            inEnemyWalkRange = false;
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
