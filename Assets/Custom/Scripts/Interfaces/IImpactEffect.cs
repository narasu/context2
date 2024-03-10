public enum ImpactType { H_SURFACE, V_SURFACE, EXPLOSION }

public interface IImpactEffect
{
    ImpactType EffectType { get; }
    float EffectTime { get; }
    void SetEffectTime(float _time);
}