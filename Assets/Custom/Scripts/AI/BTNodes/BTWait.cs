using UnityEngine;
using UnityEngine.AI;

public class BTWait : BTBaseNode
{
    private readonly float waitTime;
    
    private float t;
    
    public BTWait(float _waitTime) : base("Wait")
    {
        waitTime = _waitTime;
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        t = .0f;
    }

    protected override TaskStatus Run()
    {
        t += Time.fixedDeltaTime;
        if (t >= waitTime)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
