

public class BTCounter : BTBaseNode
{
    private readonly string variableName;
    private readonly int maxCount;
    private readonly Blackboard blackboard;
    

    public BTCounter(string _variableName, int _maxCount, Blackboard _blackboard) : base("Counter")
    {
        variableName = _variableName;
        maxCount = _maxCount;
        blackboard = _blackboard;
    }
    
    protected override TaskStatus Run()
    {
        try
        {
            int i = blackboard.GetVariable<int>(variableName);
            if (i++ >= maxCount)
            {
                i = 0;
            }
            blackboard.SetVariable(variableName, i);
            return TaskStatus.Success;
        }
        catch
        {
            return TaskStatus.Failed;
        }
    }
}