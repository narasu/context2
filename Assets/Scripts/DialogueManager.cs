using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    public GameObject DialogueObject;
    private Action<ClickEvent> toggleEventHandler;

    private void Awake()
    {
        toggleEventHandler = ToggleDialogue;
    }

    private void OnEnable()
    {
        EventManager.Subscribe(typeof(ClickEvent), toggleEventHandler);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(typeof(ClickEvent), toggleEventHandler);
    }

    private void ToggleDialogue(ClickEvent _event)
    {
        DialogueObject.SetActive(!DialogueObject.activeInHierarchy);
    }
}
