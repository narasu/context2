using UnityEngine.AI;

public class BTSetSpeed : BTBaseNode
{
    private readonly Blackboard blackboard;
    private readonly float speed;
    private NavMeshAgent agent;

    public BTSetSpeed(Blackboard _blackboard, float _speed) : base("SetSpeed")
    {
        blackboard = _blackboard;
        speed = _speed;
        agent = blackboard.GetVariable<NavMeshAgent>(Strings.Agent);
    }
    
    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        agent.speed = speed;
    }
    
    protected override TaskStatus Run()
    {
        return TaskStatus.Success;
    }
}
