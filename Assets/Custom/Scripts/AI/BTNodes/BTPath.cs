using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node sets the agent's destination to nodes along a path specified in the blackboard.
/// </summary>

public class BTPath : BTDecorator
{
    private PathNode[] pathNodes;
    private readonly Blackboard blackboard;
    private NavMeshAgent agent;
    
    private int currentNode = 0;

    public BTPath(Blackboard _blackboard, BTBaseNode _child) : base("Path", _child)
    {
        blackboard = _blackboard;
        agent = _blackboard.GetVariable<NavMeshAgent>(Strings.Agent);
        
        // convert path node positions to global
        pathNodes = _blackboard.GetVariable<List<PathNode>>(Strings.PatrolNodes).ToArray();
        for (int i = 0; i < pathNodes.Length; i++)
        {
            pathNodes[i].Position = agent.transform.position + agent.transform.rotation * pathNodes[i].Position;
            
            Vector3 sumRotation = pathNodes[i].Rotation.eulerAngles + agent.transform.rotation.eulerAngles;
            pathNodes[i].Rotation = Quaternion.Euler(sumRotation);
        }

        // add starting position as node
        IEnumerable<PathNode> r = pathNodes.Prepend(new PathNode(agent.transform.position, agent.transform.rotation));
        pathNodes = r.ToArray();
        foreach (var pn in pathNodes)
        {
            Debug.Log(pn.Position);
        }
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        blackboard.SetVariable(Strings.Destination, pathNodes[currentNode].Position);
    }

    protected override TaskStatus Run()
    {
        if (blackboard.GetVariable<Vector3>(Strings.Destination) != pathNodes[currentNode].Position)
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
