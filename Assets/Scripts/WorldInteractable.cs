using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class WorldInteractable : MonoBehaviour, IClickable
{
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void OnHoverEnter()
    {
        outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.enabled = false;
    }

    public void OnClick()
    {
        EventManager.Invoke(new ClickEvent());
    }
}
