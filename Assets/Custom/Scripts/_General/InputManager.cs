using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameInputActions InputActions => inputActions;
    private GameInputActions inputActions;
    private void Awake()
    {
        inputActions = new GameInputActions();
        inputActions.Enable();
        ServiceLocator.Provide(Strings.InputManager, this);
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}
