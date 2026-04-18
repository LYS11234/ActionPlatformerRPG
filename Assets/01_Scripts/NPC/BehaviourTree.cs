using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Running,
    Success,
    Failure
}

[System.Serializable]
public abstract class Node
{
    public abstract NodeState Evaluate();
}
[System.Serializable]
public class Sequence : Node
{
    protected List<Node> children = new List<Node>();

    public Sequence(List<Node> _children)
    {
        children = _children;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    // 하나라도 실패하면 즉시 실패 반환
                    return NodeState.Failure;
                case NodeState.Success:
                    // 성공하면 다음 노드로 계속
                    continue;
                case NodeState.Running:
                    // 하나라도 실행 중이면 전체가 실행 중
                    return NodeState.Running;
            }
        }
        // 모든 자식이 성공했을 때만 성공 반환
        return NodeState.Success;
    }

    public Sequence AddSequence(Node _node)
    {
        children.Add(_node);
        return this;
    }
}
[System.Serializable]
public class Selector : Node
{
    protected List<Node> children = new List<Node>();

    public Selector(List<Node> _children)
    {
        children = _children;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.Failure:
                    // 실패하면 다음 노드로 계속
                    continue;
                case NodeState.Success:
                    // 하나라도 성공하면 즉시 성공 반환
                    return NodeState.Success;
                case NodeState.Running:
                    // 하나라도 실행 중이면 전체가 실행 중
                    return NodeState.Running;
            }
        }
        // 모든 자식이 실패했을 때만 실패 반환
        return NodeState.Failure;
    }

    public Selector AddSelector(Node _node)
    {
        children.Add(_node);
        return this;
    }
}

#region Nodes
public class StandingNode : Node
{
    public override NodeState Evaluate()
    {
        return NodeState.Failure;
    }
}

public class CharmingNode : Node
{
    public override NodeState Evaluate()
    {
        return NodeState.Success;
    }
}

public class FollowingNode : Node
{
    private readonly PlayerController target;
    private readonly NPCController self;
    public FollowingNode(PlayerController _target, NPCController _self)
    {
        target = _target;
        self = _self;
    }
    public override NodeState Evaluate()
    {
        return NodeState.Success;
    }
}
#endregion
