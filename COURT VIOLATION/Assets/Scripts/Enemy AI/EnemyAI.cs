using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    public Ball ball;
    private Node root;

    void Start()
    {
        root = BuildTree();
    }

    void Update()
    {
        root.Evaluate();
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
        Node swingSequence = new BallInZoneDecorator(
            new BallInHitRangeDecorator(new Sequence(new List<Node> { swing }), this),
            this
        );

        Node chargeSequence = new BallInZoneDecorator(
            new PathTowardEnemyDecorator(new Sequence(new List<Node> { charge }), this),
            this
        );

        Node chaseSequence = new BallInZoneDecorator(
            new Inverter(
                new PathTowardEnemyDecorator(
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

        Node recoverStalkSequence = new EnemyHitBallDecorator(stalk, this);

        Node defaultStalkSequence = stalk;

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
        throw new NotImplementedException();
    }

    public bool IsBallInHitRange()
    {
        throw new NotImplementedException();
    }

    public NodeState Swing()
    {
        throw new NotImplementedException();
    }

    public bool IsBallPathTowardEnemy()
    {
        throw new NotImplementedException();
    }

    public NodeState Charge()
    {
        throw new NotImplementedException();
    }

    public bool IsBallInWalkDistance()
    {
        throw new NotImplementedException();
    }

    public NodeState WalkToTarget()
    {
        throw new NotImplementedException();
    }

    public NodeState SprintToTarget()
    {
        throw new NotImplementedException();
    }

    public bool HasHitBall()
    {
        throw new NotImplementedException();
    }

    public NodeState Stalk()
    {
        throw new NotImplementedException();
    }
}
