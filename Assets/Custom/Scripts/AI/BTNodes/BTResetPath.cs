using UnityEngine.AI;

public class BTResetPath : BTBaseNode
{
    private readonly Blackboard blackboard;
    private readonly NavMeshAgent agent;

    public BTResetPath(Blackboard _blackboard) : base("ResetPath")
    {
        blackboard = _blackboard;
        agent = blackboard.GetVariable<NavMeshAgent>(Strings.Agent);
    }
    
    protected override TaskStatus Run()
    {
        agent.ResetPath();
        return TaskStatus.Success;
    }
}
