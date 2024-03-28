using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreManager : Singleton<ScoreManager>
{
    
    public int MachineCounter;


    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MachineCounter = GameManager.Instance.Objectives.Count;
    }
}