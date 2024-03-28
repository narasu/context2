using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwitchImageByInputState : MonoBehaviour
{
    
    
    
    public Image MouseKeyboard;
    public Image Gamepad;
    private GameInputActions inputActions;
    private InputState state;
    

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}

public enum InputState { MouseKeyboard, Gamepad }