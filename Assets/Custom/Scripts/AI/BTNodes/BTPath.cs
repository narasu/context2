using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node sets the agent's destination to nodes along a path specified in the blackboard.
/// </summary>

public class BTPath : BTDecorator
{
    private readonly Transform[] pathNodes;
    private readonly Blackboard blackboard;
    
    private int currentNode = 0;

    public BTPath(Blackboard _blackboard, BTBaseNode _child) : base("Path", _child)
    {
        blackboard = _blackboard;
        pathNodes = _blackboard.GetVariable<GameObject>(Strings.PatrolNodes).GetComponentsInChildren<Transform>();
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        blackboard.SetVariable(Strings.Destination, pathNodes[currentNode].position);
    }

    protected override TaskStatus Run()
    {
        if (blackboard.GetVariable<Vector3>(Strings.Destination) != pathNodes[currentNode].position)
        {
            return TaskStatus.Failed;
        }
        
        TaskStatus childStatus = child.Tick();

        if (childStatus == TaskStatus.Success)
        {
            currentNode++;
            if (currentNode == pathNodes.Length)
            {
                currentNode = 0;
            }

            return TaskStatus.Success;
        }

        return childStatus;
    }
}
