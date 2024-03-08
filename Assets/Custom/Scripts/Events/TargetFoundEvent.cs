using UnityEngine;

public struct TargetFoundEvent
{
    public Transform Target { get; }

    public TargetFoundEvent(Transform _target)
    {
        Target = _target;
    }
}
