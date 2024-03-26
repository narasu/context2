using System;
using UnityEngine;

public class SlowSurface : MonoBehaviour, IImpactEffect
{

    private Timer lifetime;
    public ImpactType EffectType => ImpactType.H_SURFACE;
    public float EffectTime { get; private set; }
    public void SetEffectTime(float _time) => EffectTime = _time;
    
    private void Start()
    {
        Action onTimerExpired = () => Destroy(gameObject);
        lifetime = new Timer(EffectTime, onTimerExpired);
    }

    private void Update()
    {
        lifetime.RunTimer();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<ISlowable>()?.Slow(0.25f);
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<ISlowable>()?.Unslow();
    }
}
