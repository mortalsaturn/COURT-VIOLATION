using UnityEngine;

public class BallInZoneDecorator : ConditionDecorator
{
    private EnemyAI ai;

    public BallInZoneDecorator(Node child, EnemyAI ai)
        : base(child)
    {
        this.ai = ai;
    }

    protected override bool Condition()
    {
        return ai.IsBallInZone();
    }
}

public class BallInHitRangeDecorator : ConditionDecorator
{
    private EnemyAI ai;

    public BallInHitRangeDecorator(Node child, EnemyAI ai)
        : base(child)
    {
        this.ai = ai;
    }

    protected override bool Condition()
    {
        return ai.IsBallInHitRange();
    }
}

public class SwingNode : Node
{
    private EnemyAI ai;

    public SwingNode(EnemyAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ran " + ToString());

        return ai.OnSwing();
    }
}

public class BallApproachingEnemyDecorator : ConditionDecorator
{
    private EnemyAI ai;

    public BallApproachingEnemyDecorator(Node child, EnemyAI ai)
        : base(child)
    {
        this.ai = ai;
    }

    protected override bool Condition()
    {
        return ai.IsBallApproachingEnemy();
    }
}

public class ChargeNode : Node
{
    EnemyAI ai;

    public ChargeNode(EnemyAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ran " + ToString());

        return ai.OnCharge();
    }
}

public class BallInWalkDistanceDecorator : ConditionDecorator
{
    private EnemyAI ai;

    public BallInWalkDistanceDecorator(Node child, EnemyAI ai)
        : base(child)
    {
        this.ai = ai;
    }

    protected override bool Condition()
    {
        return ai.IsBallInWalkDistance();
    }
}

public class WalkNode : Node
{
    private EnemyAI ai;

    public WalkNode(EnemyAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ran " + ToString());

        return ai.WalkToTarget();
    }
}

public class SprintNode : Node
{
    private EnemyAI ai;

    public SprintNode(EnemyAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ran " + ToString());

        return ai.SprintToTarget();
    }
}

public class EnemyHitBallDecorator : ConditionDecorator
{
    private EnemyAI ai;

    public EnemyHitBallDecorator(Node child, EnemyAI ai)
        : base(child)
    {
        this.ai = ai;
    }

    protected override bool Condition()
    {
        return ai.HasHitBall();
    }
}

public class StalkNode : Node
{
    private EnemyAI ai;

    public StalkNode(EnemyAI ai)
    {
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Ran " + ToString());

        return ai.Stalk();
    }
}
