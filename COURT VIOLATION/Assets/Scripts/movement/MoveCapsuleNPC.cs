using UnityEngine;

public class MoveCapsuleNPC : MonoBehaviour
{

    public Transform target;
    public float moveDuration = 3f;

    public CameraManager cameraManager; 

    private Vector3 startPosition;
    private float speed;
    private bool hasReachedTarget = false;
    private Transform player;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("NPC").transform;

        startPosition = transform.position;

        float distance = Vector3.Distance(startPosition, target.position);
        speed = distance / moveDuration;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null && !hasReachedTarget)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            if (animator != null)
            {
                animator.SetBool("isrunning", true);
            }

            if (Vector3.Distance(transform.position, target.position) < 0.01f)
            {
                hasReachedTarget = true;

                if (animator != null)
                {
                    animator.SetBool("isrunning", false);
                }

                // camera switch
                if (cameraManager != null)
                {
                    
                    player.transform.Rotate(0f, -90f, 0f);
                }

                Debug.Log("Capsule has reached the target!");
            }
        }
    }
}
