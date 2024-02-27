using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Collider))]
public class WorldInteractable : MonoBehaviour
{
    private Outline outline;

    private int triggerCount;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void OnHoverEnter()
    {
        outline.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCount++;
        }

        if (triggerCount > 0)
        {
            outline.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerCount--;
        }

        if (triggerCount == 0)
        {
            outline.enabled = false;
        }
    }


    public void OnHoverExit()
    {
        outline.enabled = false;
    }

    public void OnClick()
    {
        
    }
}
