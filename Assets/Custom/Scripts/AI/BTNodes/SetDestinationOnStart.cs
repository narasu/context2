using UnityEngine;

public class SetDestinationOnStart : BTBaseNode
{
    private readonly Blackboard blackboard;

    public SetDestinationOnStart(Blackboard _blackboard) : base("ReturnToStartingPos")
    {
        blackboard = _blackboard;
    }
    
    protected override TaskStatus Run()
    {
        blackboard.SetVariable(Strings.Destination, blackboard.GetVariable<Vector3>(Strings.StartingPosition));
        return TaskStatus.Success;
    }
}
