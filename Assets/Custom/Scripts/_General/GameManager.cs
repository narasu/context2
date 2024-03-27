using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Action<ObjectiveCompletedEvent> objectiveCompletedEventHandler;
    private Action<PlayerCaughtEvent> playerCaughtEventHandler;
    private List<Objective> objectives = new();
    private int completedCount;

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
        
    }
    private void Awake()
    {
        objectiveCompletedEventHandler = OnObjectiveCompleted;
        playerCaughtEventHandler = OnPlayerCaught;
        objectives = new List<Objective>(FindObjectsOfType<Objective>());
    }
    private void OnEnable()
    {
        EventManager.Subscribe(typeof(ObjectiveCompletedEvent), objectiveCompletedEventHandler);
        EventManager.Subscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(typeof(ObjectiveCompletedEvent), objectiveCompletedEventHandler);
        EventManager.Unsubscribe(typeof(PlayerCaughtEvent), playerCaughtEventHandler);
    }
}
