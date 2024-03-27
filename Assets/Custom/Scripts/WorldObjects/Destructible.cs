using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructible : MonoBehaviour, IDamageable
{
    public GameObject Explosion;
    public UnityEvent OnTakeDamage;
    public void TakeDamage()
    {
        OnTakeDamage?.Invoke();
        ScoreManager.Instance.MachineCounter--;
        Instantiate(Explosion,transform.position,new Quaternion(0,90,0,0));
        Destroy(gameObject);
    }
}
