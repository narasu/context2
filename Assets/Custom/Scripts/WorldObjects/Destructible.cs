using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour, IDamageable
{
    public UnityEvent OnTakeDamage;
    public void TakeDamage()
    {
        OnTakeDamage?.Invoke();
        ScoreManager.Instance.MachineCounter--;
        Destroy(gameObject);
    }
}
