using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float followDistance = 10f;   
    public float stopDistance = 2f;     
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

        
        if (distance <= followDistance && distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
          
            rb.linearVelocity = Vector3.zero;
        }
    }
}
