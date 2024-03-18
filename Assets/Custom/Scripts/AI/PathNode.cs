using System;
using UnityEngine;

[Serializable]
public struct PathNode
{
    public Vector3 Position;
    public Quaternion Rotation;


    public PathNode(Vector3 _position)
    {
        Position = _position;
        Rotation = Quaternion.identity;
    }
    
    public PathNode(Vector3 _position, Quaternion _rotation)
    {
        Position = _position;
        Rotation = _rotation;
    }

    public Vector3 Forward => (Rotation * Vector3.forward).normalized;
}
