using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float followDistance = 10f;   // how far away the NPC starts chasing
    public float stopDistance = 2f;      // how close the NPC stops
    public float moveSpeed = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Only follow if:
        // 1. Player is within follow distance
        // 2. NPC is NOT too close (greater than stopDistance)
        if (distance <= followDistance && distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // keep NPC grounded

            rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Stop movement completely
            rb.linearVelocity = Vector3.zero;
        }
    }
}
