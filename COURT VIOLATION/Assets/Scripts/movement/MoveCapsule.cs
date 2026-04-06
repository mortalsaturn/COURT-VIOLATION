using UnityEngine;

public class MoveCapsule : MonoBehaviour
{
    public Transform target;
    public float moveDuration = 3f;

    private Vector3 startPosition;
    private float speed;
    private bool hasReachedTarget = false; // track if movement is done

    void Start()
    {
        startPosition = transform.position;

        float distance = Vector3.Distance(startPosition, target.position);
        speed = distance / moveDuration; // calculate speed
    }

    void Update()
    {
        if (target != null && !hasReachedTarget)
        {
            // Move the capsule toward the target
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            // Check if capsule has reached the target
            if (transform.position.z == target.position.z)
            {
                hasReachedTarget = true; // stop moving
                Debug.Log("Capsule has reached the target!");
            }
        }
    }
}