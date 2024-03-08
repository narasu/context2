using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This node moves the agent to the destination specified in the blackboard.
/// If the destination is invalid, the node will return TaskStatus.Failed.
/// If the agent reaches the destination, the node will return TaskStatus.Success.
/// </summary>

public class BTMoveTo : BTBaseNode
{
    private readonly Blackboard blackboard;
    private readonly NavMeshAgent agent;

    public BTMoveTo(Blackboard _blackboard) : base("MoveTo")
    {
        blackboard = _blackboard;
        agent = _blackboard.GetVariable<NavMeshAgent>(Strings.Agent);
    }



    protected override TaskStatus Run()
    {
        agent.SetDestination(blackboard.GetVariable<Vector3>(Strings.Destination));
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            return TaskStatus.Failed;
        }
        
        if (Vector3.Distance(agent.transform.position, agent.destination) <= agent.stoppingDistance)
        {
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }

}
