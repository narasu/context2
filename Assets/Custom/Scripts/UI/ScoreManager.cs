using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreManager : Singleton<ScoreManager>
{
    
    public int MachineCounter = 5;


    void Awake()
    {
        Instance = this;
        
    }
}