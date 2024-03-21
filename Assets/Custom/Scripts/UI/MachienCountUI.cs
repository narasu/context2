using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class CoinDisplay : MonoBehaviour
{
    private TMP_Text coin;

    private void Awake()
    {
        coin = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        coin.text = ScoreManager.Instance.MachineCounter.ToString();
    }
}

