using System;

/// <summary>
/// This decorator node inverts the result of its child node.
/// </summary>

public class BTInvert : BTDecorator
{
    public BTInvert(BTBaseNode _child) : base("Invert", _child) { }

    protected override TaskStatus Run()
    {
        TaskStatus childStatus = child.Tick();

        switch (childStatus)
        {
            case TaskStatus.Success:
                return TaskStatus.Failed;
            case TaskStatus.Failed:
                return TaskStatus.Success;
            default:
                return childStatus;
        }
    }
}
