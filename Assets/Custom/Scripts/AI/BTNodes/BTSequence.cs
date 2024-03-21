using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTComposite
{
    private readonly bool keepPosition;
    private readonly int repeatCount;
    private int currentIndex;

    
    public BTSequence(string _name, params BTBaseNode[] _children) : base(_name, _children)
    {
        name = _name;
    }
    
    public BTSequence(string _name, bool _keepPosition, params BTBaseNode[] _children) : base(_name, _children)
    {
        keepPosition = _keepPosition;
        name = _name;
    }
    
    public BTSequence(string _name, bool _keepPosition, int _repeatCount, params BTBaseNode[] _children) : base(_name, _children)
    {
        keepPosition = _keepPosition;
        repeatCount = _repeatCount;
        name = _name;
    }

    protected override TaskStatus Run()
    {
        for (; currentIndex < children.Length; currentIndex++)
        {
            TaskStatus childStatus = children[currentIndex].Tick();

            switch (childStatus)
            {
                case TaskStatus.Running:
                    return TaskStatus.Running;
                case TaskStatus.Failed:
                    if (!keepPosition)
                    {
                        currentIndex = 0;
                    }
                    return TaskStatus.Failed;
                case TaskStatus.Success:
                    break;
            }
        }

        if (!keepPosition)
        {
            currentIndex = 0;
        }

        return TaskStatus.Success;
    }
    
}