using System;

public class BTGetStatus : BTBaseNode
{
    private readonly Blackboard blackboard;
    private readonly string statusString;

    public BTGetStatus(Blackboard _blackboard, string _statusString) : base("GetStatus")
    {
        blackboard = _blackboard;
        statusString = _statusString;
    }

    protected override TaskStatus Run()
    {
        return blackboard.GetVariable<TaskStatus>(statusString);
    }
}