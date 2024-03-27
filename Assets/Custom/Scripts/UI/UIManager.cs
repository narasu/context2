using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject Disclaimer;
    public GameObject Tutorial;
    public GameObject[] IDCards;
    public GameObject ObjectiveCounter;
    public GameObject[] CaughtScreens;

    private GameInputActions inputActions;
    private GameObject[] sequence;
    private int currentScreen = 0;
    private Action<ObjectiveCompletedEvent> objectiveCompletedEventHandler;
    private Action<PlayerCaughtEvent> playerCaughtEventHandler;

    private void Awake()
    {
        playerCaughtEventHandler = OnPlayerCaught;
    }

    private void OnEnable()
    {
        inputActions = new GameInputActions();
        inputActions.Enable();
        inputActions.Player.Jump.performed += Next;
        sequence = new[]
        {
            Disclaimer,
            Tutorial,
            IDCards[0]
        };
        sequence[0].SetActive(true);
        EventManager.Subscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
        
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= Next;
        inputActions.Disable();
        EventManager.Unsubscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
    }

    private void OnPlayerCaught(PlayerCaughtEvent _event)
    {
        CaughtScreens[0].SetActive(true);
    }
    
    private void Next(InputAction.CallbackContext _ctx)
    {
        sequence[currentScreen].SetActive(false);
        if (currentScreen < sequence.Length - 1)
        {
            currentScreen++;
            sequence[currentScreen].SetActive(true);
        }
        else
        {
            ObjectiveCounter.SetActive(true);
            EventManager.Invoke(new GameStartedEvent());
        }
    }
}
