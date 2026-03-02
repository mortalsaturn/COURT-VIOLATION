using UnityEngine;
using UnityEngine.UIElements;

public class SimpleEnemyAI : MonoBehaviour
{
    [SerializeField]
    private NewCharacterStats stats;
    public Ball ballScript;
    public Transform ballTransform;
    public float baseSpeed;
    public float sprintModifier;
    public float swingDistance;
    Hitbox[] hitboxes;
    Animator animator;
    bool inEnemyZone;
    public Collider enemyZone;

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
            Move();
            Swing(stats.shotBaseVel);
        }
        else
        {
            if (ballScript.lastHitter != "Enemy")
            {
                Move();
            }
        }
    }

    void Move()
    {
        var moveCondition = ballTransform.position.z > enemyZone.bounds.min.z;
        Vector3 target = new(
            ballTransform.position.x - 1.2f,
            transform.position.y,
            ballTransform.position.z + 1.5f
        );
        if (moveCondition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                baseSpeed * Time.deltaTime
            );
        }
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

    void OnTriggerStay(Collider trigger)
    {
        inEnemyZone = trigger.gameObject.layer == LayerMask.NameToLayer("EnemyZone");
    }
}
