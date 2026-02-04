using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public enum NodeState
{
    Success, // complete its process
    Running, // still in progress
    Failure, // fail its process
}

/// <summary>
/// The building blocks of a tree structure, housing data and defining the hierarchical relationships that characterize a tree graph.
/// </summary>
public abstract class Node
{
    public NodeState state;

    /// <summary>
    /// This function is called every frame (typically through Update).
    /// </summary>
    public abstract NodeState Evaluate(); // must be implemented and is called every frame
}

/// <summary>
/// A Sequence tries each child in order.
/// It’s a “do A then B then C” structure.
/// </summary>
public class Sequence : Node
{
    private List<Node> children;

    public Sequence(List<Node> children)
    {
        this.children = children;
    }

    /// <summary>
    /// This loops through all children.
    /// If any child fails, the whole sequence fails immediately.
    /// </summary>
    public override NodeState Evaluate()
    {
        bool anyRunning = false;

        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;

                case NodeState.Running:
                    anyRunning = true;
                    continue;
            }
        }

        state = anyRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}

/// <summary>
/// A Selector is basically “fall back to the next option”. “Try A; if A fails, try B; if B fails, try C...”.
/// </summary>
public class Selector : Node
{
    private List<Node> children;

    public Selector(List<Node> children)
    {
        this.children = children;
    }

    /// <summary>
    /// A child returning Success stops the Selector and the Selector succeeds. Only if all children fail does the Selector return Failure.
    /// </summary>
    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;

                case NodeState.Running:
                    state = NodeState.Running;
                    return state;
            }
        }

        state = NodeState.Failure;
        return state;
    }
}

/// <summary>
/// Runs the child only if a condition is true.
/// </summary>
public abstract class ConditionDecorator : Node
{
    protected Node child;

    public ConditionDecorator(Node child)
    {
        this.child = child;
    }

    // Must be implemented in subclasses
    protected abstract bool Condition();

    public override NodeState Evaluate()
    {
        if (!Condition())
        {
            state = NodeState.Failure;
            return state;
        }

        return child.Evaluate();
    }
}

public class Inverter : Node
{
    private Node child;

    public Inverter(Node child)
    {
        this.child = child;
    }

    public override NodeState Evaluate()
    {
        NodeState result = child.Evaluate();

        switch (result)
        {
            case NodeState.Success:
                return NodeState.Failure;
            case NodeState.Failure:
                return NodeState.Success;
            case NodeState.Running:
                return NodeState.Running;
            default:
                return NodeState.Failure;
        }
    }
}

/// <summary>
/// Always returns Success, even if the child fails.
/// This is used when you always want the parent to continue evaluating the next branch.
/// </summary>
public class AlwaysTrueDecorator : Node
{
    private Node child;

    public AlwaysTrueDecorator(Node child)
    {
        this.child = child;
    }

    /// <returns>NodeState.Success</returns>
    public override NodeState Evaluate()
    {
        child.Evaluate();
        state = NodeState.Success;
        return state;
    }
}

public class WhileDecorator : Node
{
    private readonly Func<bool> _condition;
    private readonly Node _child;

    public WhileDecorator(Func<bool> condition, Node child)
    {
        _condition = condition;
        _child = child;
    }

    public override NodeState Evaluate()
    {
        if (!_condition())
        {
            // Condition false, loop ended normally
            return NodeState.Success;
        }

        var result = _child.Evaluate();

        if (result == NodeState.Failure)
        {
            // Child failure, loop aborted
            return NodeState.Failure;
        }

        // Child is running or succeeded, continue looping
        return NodeState.Running;
    }
}
