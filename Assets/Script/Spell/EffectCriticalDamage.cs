using UnityEngine;
using System.Collections;

public class EffectCriticalDamage : BaseBulletEffect
{
    public float _minDamage;
    public float _maxDamage;
    public float _criticalPercent;

    public override void TriggerEffect(GameObject targetGO)
    {
        if (targetGO.layer == 8 || targetGO.layer == 9)
        {
            ActorHealth health = targetGO.GetComponent<ActorHealth>();
            health.DecreaseHealth(MathUtils.RandomBoolean(_criticalPercent) ? Random.Range(_minDamage, _maxDamage) : _minDamage);
        }
    }
}
