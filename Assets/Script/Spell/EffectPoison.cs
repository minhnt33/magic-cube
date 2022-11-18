using UnityEngine;
using System.Collections;

public class EffectPoison : BaseBulletEffect
{
    public float _damage;
    public float _duration;

    public delegate void OnPoisionEffect(GameObject target, float duration);
    public static event OnPoisionEffect OnPoisionEffectEvent;

    public override void TriggerEffect(GameObject targetGO)
    {
        ActorHealth targetHealth = targetGO.GetComponent<ActorHealth>();

        if (targetHealth)
        {
            targetHealth.DecreaseHealthInSeconds(_damage, _duration);


            if (OnPoisionEffectEvent != null)
                OnPoisionEffectEvent(targetGO, _duration);
        }
    }
}
