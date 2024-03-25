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
        rb.mass = data.Mass;
    }
    
    public void Throw(Vector3 _direction)
    {
        rb.AddForce(_direction * data.ThrowForce, ForceMode.Impulse);
    }

    public void OnImpact(Collision _collision)
    {
        IDamageable damageable = _collision.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage();
            Destroy(gameObject);
            return;
        }
        
        Vector3 pos = _collision.GetContact(0).point;
        if (data.SpawnOnImpact != null)
        {
            var e = data.SpawnOnImpact.GetComponent<IImpactEffect>();
            GameObject impact = null;
            
            if (e.EffectType == ImpactType.H_SURFACE && _collision.GetContact(0).normal.y != 0)
            {
                impact = Instantiate(data.SpawnOnImpact, pos, Quaternion.identity);
            }
            else if (e.EffectType == ImpactType.V_SURFACE && _collision.GetContact(0).normal.x != 0)
            {
                impact = Instantiate(data.SpawnOnImpact, pos, Quaternion.identity);
            }
            
            if (impact != null)
            {
                impact.GetComponent<IImpactEffect>()?.SetEffectTime(data.ImpactEffectTime);
            }
        }
        Destroy(gameObject);
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
        OnImpact(other);
    }
}
