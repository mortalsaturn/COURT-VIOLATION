using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private NewCharacterStats stats;
    private Node root;
    private Node swingSequence;
    private Node chargeSequence;
    private Node chaseSequence;
    private Node recoverStalkSequence;
    private Node defaultStalkSequence;

    [Header("Ball")]
    public Transform ballTransform;
    public Ball ballScript;

    [Header("Hitboxes")]
    Hitbox[] hitboxes;

    [Header("Charge")]
    float addedVelocity;
    float chargeRate;
    public Coroutine chargeRoutine;
    Animator animator;
    public Transform baseline;

    [Header("Movement")]
    public float baseSpeed;
    public float sprintModifier;
    private bool _hasStartedSwing;

    void Start()
    {
        animator = GetComponent<Animator>();
        hitboxes = GetComponentsInChildren<Hitbox>();
        root = BuildTree();
    }

    void Update()
    {
        // TEMPORARY
        Debug.Log("SwingSequence: " + swingSequence.Evaluate());
        Debug.Log("ChargeSequence: " + chargeSequence.Evaluate());
        Debug.Log("ChaseSequence: " + chaseSequence.Evaluate());
        Debug.Log("RecoverStalk: " + recoverStalkSequence.Evaluate());
        Debug.Log("DefaultStalk: " + defaultStalkSequence.Evaluate());
    }

    private Node BuildTree()
    {
        // Leaf Nodes
        Node swing = new SwingNode(this);
        Node charge = new ChargeNode(this);
        Node walkToTarget = new WalkNode(this);
        Node sprintToTarget = new SprintNode(this);
        Node stalk = new StalkNode(this);

        // Decorated Actions
        swingSequence = new BallInZoneDecorator(new BallInHitRangeDecorator(swing, this), this);

        chargeSequence = new BallInZoneDecorator(
            new BallApproachingEnemyDecorator(
                new WhileDecorator(() => !ballScript.inEnemyHitRange, charge),
                this
            ),
            this
        );

        chaseSequence = new BallInZoneDecorator(
            new Inverter(
                new BallApproachingEnemyDecorator(
                    new Selector(
                        new List<Node>
                        {
                            new BallInWalkDistanceDecorator(walkToTarget, this),
                            new AlwaysTrueDecorator(sprintToTarget),
                        }
                    ),
                    this
                )
            ),
            this
        );

        recoverStalkSequence = new EnemyHitBallDecorator(stalk, this);

        defaultStalkSequence = stalk;

        // Root Selector
        return new Selector(
            new List<Node>
            {
                swingSequence,
                chargeSequence,
                chaseSequence,
                recoverStalkSequence,
                defaultStalkSequence,
            }
        );
    }

    public bool IsBallInZone()
    {
        return ballScript.inEnemyZone;
    }

    public bool IsBallInHitRange()
    {
        return ballScript.inEnemyHitRange;
    }

    public NodeState OnSwing()
    {
        float vel = Mathf.Min(stats.basicShotBaseVel + addedVelocity, stats.basicShotMaxVel);
        NodeState result = Swing(vel);

        // If swing finished, clear charge state
        if (result == NodeState.Success)
        {
            if (chargeRoutine != null)
            {
                StopCoroutine(chargeRoutine);
                chargeRoutine = null;
            }

            addedVelocity = 0;
        }

        return result;
    }

    public NodeState Swing(float velocity)
    {
        foreach (var hb in hitboxes)
            hb.launchVel = velocity;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool inLeft = stateInfo.IsName("LeftBasicSwing");
        bool inRight = stateInfo.IsName("RightBasicSwing");

        // If we haven't started the swing yet, trigger it once
        if (!_hasStartedSwing)
        {
            _hasStartedSwing = true;
            animator.SetBool("isBallRight", ballScript.destination.x >= transform.position.x);
            animator.SetTrigger("Swing");
            return NodeState.Running;
        }

        // If we're in the swing state, wait for it to finish
        if (inLeft || inRight)
        {
            if (stateInfo.normalizedTime < 1f)
                return NodeState.Running;

            // finished
            _hasStartedSwing = false;
            return NodeState.Success;
        }

        // If we're not in the swing states yet (still transitioning), keep waiting
        return NodeState.Running;
    }

    public bool IsBallApproachingEnemy()
    {
        return ballScript.isApproachingEnemy;
    }

    public NodeState OnCharge()
    {
        if (chargeRoutine == null)
        {
            chargeRoutine = StartCoroutine(Charge());
        }

        return NodeState.Running;
    }

    private IEnumerator Charge()
    {
        while (true)
        {
            addedVelocity += chargeRate * Time.deltaTime;
            yield return null;
        }
    }

    public bool IsBallInWalkDistance()
    {
        return ballScript.inEnemyWalkRange;
    }

    public NodeState WalkToTarget()
    {
        if (!ballScript.inEnemyHitRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                ballTransform.position,
                baseSpeed * Time.deltaTime
            );
            return NodeState.Running;
        }
        else
        {
            return NodeState.Success;
        }
    }

    public NodeState SprintToTarget()
    {
        if (!ballScript.inEnemyHitRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                ballTransform.position,
                baseSpeed * sprintModifier * Time.deltaTime
            );
            return NodeState.Running;
        }
        else
        {
            return NodeState.Success;
        }
    }

    public bool HasHitBall()
    {
        return ballScript.lastHitter == gameObject.name;
    }

    public NodeState Stalk()
    {
        Vector3 currentPos = transform.position;
        float targetX = ballTransform.position.x;
        float targetZ = baseline.position.z;

        Vector3 targetPos = new Vector3(targetX, currentPos.y, targetZ);

        if (Vector3.Distance(currentPos, targetPos) > 2f)
        {
            transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                baseSpeed * Time.deltaTime
            );
            return NodeState.Running;
        }
        else
        {
            return NodeState.Success;
        }
    }
}
