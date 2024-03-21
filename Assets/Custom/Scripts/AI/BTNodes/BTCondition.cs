using System;

/// <summary>
/// This decorator node runs its child only if a condition is met
/// </summary>

public class BTCondition : BTDecorator
{
    private bool condition;
    private readonly BTBaseNode onTrue;
    private TaskStatus onFalse;

    public BTCondition(bool _condition, BTBaseNode _onTrue, TaskStatus _onFalse) : base("condition", _onTrue)
    {
        condition = _condition;
        onTrue = _onTrue;
        onFalse = _onFalse;
    }

    protected override TaskStatus Run()
    {

        TaskStatus status = condition ? onTrue.Tick() : onFalse;

        return status;
    }
}
