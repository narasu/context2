using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// This node sets the destination on the blackboard to the position of the target, if there is one.
/// If there is no target, the node will return TaskStatus.Failed.
/// </summary>
public class BTSetDestinationOnTarget : BTBaseNode
{
    private readonly Blackboard blackboard;

    public BTSetDestinationOnTarget(Blackboard _blackboard) : base("SetDestinationOnTarget")
    {
        blackboard = _blackboard;
    }
    
    protected override TaskStatus Run()
    {
        Transform target = blackboard.GetVariable<Transform>(Strings.Target);
        
        if (target == null)
        {
            return TaskStatus.Failed;
        }

        blackboard.SetVariable(Strings.Destination, target.position);
        return TaskStatus.Success;
    }
}
