using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Action<ObjectiveCompletedEvent> objectiveCompletedEventHandler;
    private Action<GameStartedEvent> gameStartedEventHandler;
    private Action<PlayerCaughtEvent> playerCaughtEventHandler;
    private List<Objective> objectives = new();
    private List<Guard> guards = new();
    private MovementController player;
    private int completedCount;

    private int currentRun;

    private void OnObjectiveCompleted(ObjectiveCompletedEvent _event)
    {
        completedCount++;
        if (completedCount >= objectives.Count)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Win!");
        }
    }

    private void OnPlayerCaught(PlayerCaughtEvent _event)
    {
        player.Reset();
        player.gameObject.SetActive(false);
        foreach (var guard in guards)
        {
            guard.Reset();
            guard.gameObject.SetActive(false);
        }

        currentRun++;
    }
    
    private void OnGameStarted(GameStartedEvent _event)
    {
        SoundManager.instance.PlayMusic();
        player.gameObject.SetActive(true);
        foreach (var guard in guards)
        {
            guard.gameObject.SetActive(true);
        }
    }
    
    private void Awake()
    {
        objectiveCompletedEventHandler = OnObjectiveCompleted;
        playerCaughtEventHandler = OnPlayerCaught;
        gameStartedEventHandler = OnGameStarted;
        objectives = new List<Objective>(FindObjectsOfType<Objective>());
        guards = new List<Guard>(FindObjectsOfType<Guard>());
        foreach (var guard in guards)
        {
            guard.gameObject.SetActive(false);
        }
        player = FindObjectOfType<MovementController>();
        player.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        EventManager.Subscribe(typeof(ObjectiveCompletedEvent), objectiveCompletedEventHandler);
        EventManager.Subscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
        EventManager.Subscribe(typeof(GameStartedEvent), gameStartedEventHandler);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(typeof(ObjectiveCompletedEvent), objectiveCompletedEventHandler);
        EventManager.Unsubscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
        EventManager.Unsubscribe(typeof(GameStartedEvent), gameStartedEventHandler);
    }
}
