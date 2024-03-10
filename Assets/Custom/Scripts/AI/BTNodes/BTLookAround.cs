
using UnityEngine;

public class BTLookAround : BTBaseNode
{
    private readonly Blackboard blackboard;
    private Transform viewTransform;
    private Animation anim;

    public BTLookAround(Blackboard _blackboard) : base("LookAround")
    {
        blackboard = _blackboard;
        viewTransform = _blackboard.GetVariable<Transform>(Strings.ViewTransform);
        anim = viewTransform.GetComponent<Animation>();
    }

    protected override void OnEnter(bool _debug)
    {
        base.OnEnter(_debug);
        anim.Play("ViewRotate");
    }

    protected override TaskStatus Run()
    {
        if (!anim.isPlaying)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public override void OnExit(TaskStatus _status)
    {
        base.OnExit(_status);
        anim.Stop();
    }

    public override void OnTerminate()
    {
        base.OnTerminate();
        anim.Stop();
    }
}

