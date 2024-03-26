public class BTSetAgentState : BTBaseNode
{
    private readonly Blackboard blackboard;
    private readonly AgentState state;

    public BTSetAgentState(Blackboard _blackboard, AgentState _state) : base("SetAgentState")
    {
        blackboard = _blackboard;
        state = _state;
    }
    protected override TaskStatus Run()
    {
        blackboard.SetVariable(Strings.AgentState, state);
        return TaskStatus.Success;
    }
}

public enum AgentState { IDLE, PATROL, CHASE }