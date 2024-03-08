/// <summary>
/// Decorator node that caches the status of its child node on the blackboard.
/// The status is stored in the blackboard under the name specified in the constructor.
/// </summary>

public class BTCacheStatus : BTDecorator
{
    private readonly Blackboard blackboard;
    private readonly string statusString;

    public BTCacheStatus(Blackboard _blackboard, string _statusString, BTBaseNode _child) : base("CacheStatus", _child)
    {
        blackboard = _blackboard;
        statusString = _statusString;
    }
    protected override TaskStatus Run()
    {
        TaskStatus childStatus = child.Tick();
        blackboard.SetVariable<TaskStatus>(statusString, childStatus);
        return childStatus;
    }
}
