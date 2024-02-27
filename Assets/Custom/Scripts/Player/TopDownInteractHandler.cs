using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownInteractHandler : MonoBehaviour
{
    private Camera cam;
    private List<IInteractable> availableTargets = new();
    private IInteractable activeTarget;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // float distanceToActive = Vector3.Distance(transform.position, activeTarget.Position)
        // if (Input.GetKeyDown(KeyCode.E) && availableTargets.Count > 0)
        // {
        //     availableTargets[0].Interact();
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable target = other.GetComponent<IInteractable>();
        if (target != null && !availableTargets.Contains(target))
        {
            availableTargets.Add(other.GetComponent<IInteractable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable target = other.GetComponent<IInteractable>();
        if (target != null && availableTargets.Contains(target))
        {
            availableTargets.Remove(target);
        }
    }
}
