using System;
using UnityEngine;

[Serializable]
public struct PathNode
{
    public Vector3 Position;
    public Quaternion Rotation;
    public float WaitTime;

    public PathNode(Vector3 _position)
    {
        Position = _position;
        Rotation = Quaternion.identity;
        WaitTime = .0f;
    }
    
    public PathNode(Vector3 _position, Quaternion _rotation)
    {
        Position = _position;
        Rotation = _rotation;
        WaitTime = .0f;
    }

    public Vector3 Forward => (Rotation * Vector3.forward).normalized;
}
