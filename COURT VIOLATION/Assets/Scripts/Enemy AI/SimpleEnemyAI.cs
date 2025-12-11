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
        if (Vector3.Distance(transform.position, ballTransform.position) <= swingDistance)
        {
            Swing(hitboxes[0].baseVel);
        }
        else if (ballScript.inEnemyZone)
        {
            Move();
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            ballScript.predictedPoint,
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
