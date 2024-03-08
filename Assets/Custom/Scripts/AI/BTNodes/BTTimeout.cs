using UnityEngine;

public class BTTimeout : BTDecorator
{
    private readonly float timeoutTime;
    private readonly TaskStatus onTimeout;
    private float currentTime = .0f;
    
    public BTTimeout(float _timeoutTime, TaskStatus _onTimeout, BTBaseNode _child) : base("Timeout", _child)
    {
        timeoutTime = _timeoutTime;
        onTimeout = _onTimeout;
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        currentTime = .0f;
    }

    protected override TaskStatus Run()
    {
        TaskStatus childStatus = child.Tick();

        if (childStatus == TaskStatus.Success) return TaskStatus.Success;

        if (childStatus == TaskStatus.Running)
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= timeoutTime)
            {
                return onTimeout;
            }

            return TaskStatus.Running;
        }

        return childStatus;
    }
}
