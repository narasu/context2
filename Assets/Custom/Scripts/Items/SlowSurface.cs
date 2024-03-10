using System;
using UnityEngine;

[RequireComponent(typeof(TriggerCheck))]
public class SlowSurface : MonoBehaviour, IImpactEffect
{

    private TriggerCheck triggerCheck;
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

}
