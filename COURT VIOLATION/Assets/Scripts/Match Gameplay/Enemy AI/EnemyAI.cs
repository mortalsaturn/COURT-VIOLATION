using UnityEngine;

[RequireComponent(typeof(Swing))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Ball ball;
    public Transform homePosition;

    private Swing swing;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.25f;

    [Header("Decision Making")]
    public float swingDistance = 2f;
    public float smashHeight = 3f;

    private Vector3 targetPosition;

    private float nextSwingTime;
    public float swingCooldown = 0.75f;

    void Awake()
    {
        swing = GetComponent<Swing>();
    }

    void Update()
    {
        if (ball == null)
            return;

        HandleMovement();
        HandleSwinging();
    }

    private void HandleMovement()
    {
        // Return home when ball is moving away
        if (!ball.inEnemyZone)
        {
            targetPosition = homePosition.position;
        }
        else
        {
            targetPosition = new(
                ball.transform.position.x,
                transform.position.y,
                ball.transform.position.z + 2f
            );
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stoppingDistance)
            return;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleSwinging()
    {
        if (Time.time < nextSwingTime)
            return;

        float distanceToBall = Vector3.Distance(transform.position, ball.transform.position);

        if (distanceToBall > swingDistance)
            return;

        // Face the ball
        /* Vector3 lookDir = ball.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            transform.forward = lookDir.normalized;
        } */

        DecideShot();
    }

    private void DecideShot()
    {
        // Prevent enemy from hitting own shot repeatedly
        if (ball.lastHitter == gameObject.name)
            return;

        // Randomized shot selection
        int choice = Random.Range(0, 10);

        if (choice <= 6)
        {
            swing.AI_BasicSwing();
        }
        else if (choice <= 8)
        {
            swing.AI_Lob();
        }
        else
        {
            swing.AI_DropShot();
        }
        nextSwingTime = Time.time + swingCooldown;
    }
}
