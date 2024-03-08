using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTComposite
{
    private int currentIndex = 0;

    public BTSelector(string _name, params BTBaseNode[] _children) : base(_name, _children)
    {
    }

    protected override TaskStatus Run()
    {
        for(; currentIndex < children.Length; currentIndex++)
        {
            TaskStatus childStatus = children[currentIndex].Tick();
            
            switch(childStatus)
            {
                case TaskStatus.Running:
                    return TaskStatus.Running;
                case TaskStatus.Success:
                    currentIndex = 0;
                    return TaskStatus.Success;
                case TaskStatus.Failed: 
                    break;
            }
        }
        currentIndex = 0;
        return TaskStatus.Failed;
    }
}