using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameInputActions inputActions;
    private void Awake()
    {
        inputActions = new GameInputActions();
        ServiceLocator.Provide(Strings.InputAsset, inputActions);
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
