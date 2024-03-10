using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    private bool isCompleted;
    public void Complete()
    {
        if (isCompleted)
        {
            return;
        }
        isCompleted = true;
        EventManager.Invoke(new ObjectiveCompletedEvent(this));
    }
}
