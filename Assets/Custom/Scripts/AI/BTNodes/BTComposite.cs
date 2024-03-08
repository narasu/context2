using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any BT node with multiple children.
/// </summary>

public abstract class BTComposite : BTBaseNode
{
    protected readonly BTBaseNode[] children;

    protected BTComposite(string _name, params BTBaseNode[] _children) : base(_name)
    {
        children = _children;
    }
    
    public override void OnTerminate()
    {
        base.OnTerminate();
        foreach (BTBaseNode n in children)
        {
            n.OnTerminate();
        }
    }
}