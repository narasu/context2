using System;
using UnityEngine;

/// <summary>
/// This node listens to detection events and updates the blackboard with the detected target.
/// If no target is detected, the node will return TaskStatus.Running.
/// If a target is detected, the node will return TaskStatus.Success.
/// </summary>

public class BTDetect : BTBaseNode
{
    private readonly Blackboard blackboard;
    private ViewCone viewCone;
    
    private bool hasTarget;
   

    public BTDetect(Blackboard _blackboard) : base("Detect")
    {
        blackboard = _blackboard;
        viewCone = _blackboard.GetVariable<ViewCone>(Strings.ViewCone);

        viewCone.OnTargetFound += OnTargetFound;
        viewCone.OnTargetLost += OnTargetLost;
    }
   
    public override void OnTerminate()
    {
        base.OnTerminate();
        viewCone.OnTargetFound -= OnTargetFound;
        viewCone.OnTargetLost -= OnTargetLost;
    }

    protected override TaskStatus Run()
    {
        return hasTarget ? TaskStatus.Success : TaskStatus.Running;
    }

    private void OnTargetFound(TargetFoundEvent _event)
    {
        hasTarget = true;
       
        Debug.Log(_event.Target);
        blackboard.SetVariable(Strings.Target, _event.Target);
    }
   
    private void OnTargetLost()
    {
        blackboard.SetVariable<Transform>(Strings.Target, null);
        hasTarget = false;
    }
}
