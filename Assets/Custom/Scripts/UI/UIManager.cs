using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject ButtonMapping;
    public GameObject Tutorial;
    public GameObject[] IDCards;
    public GameObject ObjectiveCounter;
    public GameObject[] CaughtScreens;

    private GameInputActions inputActions;
    private GameObject[] sequence;
    private int currentScreen = 0;
    private int currentRun;
    private bool gameStarted = false;
    private bool caught = false;
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
            ButtonMapping,
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
        caught = true;
        gameStarted = false;
        CaughtScreens[currentRun].SetActive(true);
        ObjectiveCounter.SetActive(false);
        currentRun++;
    }
    
    private void Next(InputAction.CallbackContext _ctx)
    {
        if (!gameStarted && !caught)
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
                gameStarted = true;
            }
        }

        if (caught)
        {
            if (GameManager.Instance.currentRun == 2)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                return;
            }
            // ObjectiveCounter.SetActive(true);
            // EventManager.Invoke(new GameStartedEvent());
            CaughtScreens[0].SetActive(false);
            currentScreen = 2;
            sequence[2] = IDCards[1];
            sequence[2].SetActive(true);
            caught = false;
        }
    }
}
