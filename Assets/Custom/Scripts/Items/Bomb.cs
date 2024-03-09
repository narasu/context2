using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour, IThrowable, IImpactor
{
    private Rigidbody rb;
    private BombData data;

    public void Initialize(BombData _data)
    {
        data = _data;
    }
    
    public void Throw(Vector3 _direction)
    {
        rb.AddForce(_direction * data.ThrowForce);
    }

    public void OnImpact(Collision _collision)
    {
        Vector3 pos = _collision.GetContact(0).point;
        if (data.SpawnOnImpact != null)
        {
            GameObject impact = Instantiate(data.SpawnOnImpact, pos, Quaternion.identity);
            impact.GetComponent<IImpactEffect>()?.SetEffectTime(data.ImpactEffectTime);
        }
        Destroy(gameObject);
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        OnImpact(other);
    }
}
