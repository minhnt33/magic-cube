using UnityEngine;
using System.Collections;

public class EffectDealDamage : BaseBulletEffect
{
    [SerializeField]
    private float _damage;

    public override void TriggerEffect(GameObject targetGO)
    {
        if (targetGO.layer == 8 || targetGO.layer == 9)
        {
            ActorHealth health = targetGO.GetComponent<ActorHealth>();
            health.DecreaseHealth(_damage);
        }
    }
}
