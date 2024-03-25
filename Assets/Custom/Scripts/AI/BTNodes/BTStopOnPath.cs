
using UnityEngine;

public class BTStopOnPath : BTBaseNode
{
    private readonly Blackboard blackboard;
    private Transform viewTransform;
    private Animation anim;
    private float waitTime;
    private PathNode[] patrolNodes;
    private int patrolNodeIndex;
    private float t;

    public BTStopOnPath(Blackboard _blackboard) : base("StopOnPath")
    {
        blackboard = _blackboard;
        viewTransform = _blackboard.GetVariable<Transform>(Strings.ViewTransform);
        anim = viewTransform.GetComponent<Animation>();
        patrolNodes = _blackboard.GetVariable<PathNode[]>(Strings.PatrolNodes);
        anim.Play("ViewRotate");
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        patrolNodeIndex = blackboard.GetVariable<int>(Strings.PatrolNodeIndex);
        waitTime = patrolNodes[patrolNodeIndex].WaitTime;
        t = .0f;
        foreach (AnimationState a in anim)
        {
            a.speed = 1.0f;
        }
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

    public override void OnExit(TaskStatus _status)
    {
        base.OnExit(_status);
        foreach (AnimationState a in anim)
        {
            a.time = .0f;
            a.speed = .0f;
        }
    }

    public override void OnTerminate()
    {
        base.OnTerminate();
        foreach (AnimationState a in anim)
        {
            a.time = .0f;
            a.speed = .0f;
        }
    }
}

