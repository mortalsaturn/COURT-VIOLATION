using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public Ball ballScript;
    public Transform ballTransform;
    public float baseSpeed;
    public float sprintModifier;
    public float swingDistance;
    Hitbox[] hitboxes;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        hitboxes = GetComponentsInChildren<Hitbox>();
    }

    void Update()
    {
        var ballDist = Vector3.Distance(transform.position, ballTransform.position);

        if (ballDist <= swingDistance)
        {
            Debug.Log("Distance from ball: " + ballDist);
            Debug.Log("Swinging");

            Swing(hitboxes[0].baseVel);
        }
        else
        {
            if (ballScript.lastHitter == "Player")
            {
                Move();
            }
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            new(
                ballScript.predictedPoint.x - 1.2f,
                transform.position.y,
                ballScript.predictedPoint.z + 5f
            ),
            baseSpeed * Time.deltaTime
        );
    }

    void Swing(float velocity)
    {
        foreach (var hb in hitboxes)
        {
            hb.launchVel = velocity;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("LeftBasicSwing") && !stateInfo.IsName("RightBasicSwing"))
        {
            animator.SetTrigger("Swing");
            animator.SetBool("isBallRight", ballScript.destination.x < transform.position.x);
        }
    }
}
