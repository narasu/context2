
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
        
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        patrolNodeIndex = blackboard.GetVariable<int>(Strings.PatrolNodeIndex);
        waitTime = patrolNodes[patrolNodeIndex].WaitTime;
        t = .0f;
        anim.Play("ViewRotate");
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
        anim.Rewind();
        anim.Stop();
        viewTransform.localRotation = Quaternion.identity;
    }

    public override void OnTerminate()
    {
        base.OnTerminate();
        anim.Rewind();
        anim.Stop();
        viewTransform.localRotation = Quaternion.identity;
    }
}

