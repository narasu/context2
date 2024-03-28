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
    public List<Objective> Objectives = new();
    private List<Guard> guards = new();
    private MovementController player;
    private int completedCount;

    public int currentRun;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    private static GameManager instance;
    
    
    private void OnObjectiveCompleted(ObjectiveCompletedEvent _event)
    {
        completedCount++;
        if (completedCount >= Objectives.Count)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Win!");
        }
    }

    private void OnPlayerCaught(PlayerCaughtEvent _event)
    {
        SoundManager.instance.StopMusic();
        SoundManager.instance.PlaySoundClip(SoundManager.instance.Arrest, transform, 1.0f);
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
        Objectives = new List<Objective>(FindObjectsOfType<Objective>());
        guards = new List<Guard>(FindObjectsOfType<Guard>());
        player = FindObjectOfType<MovementController>();
    }

    private void Start()
    {
        foreach (var guard in guards)
        {
            guard.gameObject.SetActive(false);
        }
        
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
