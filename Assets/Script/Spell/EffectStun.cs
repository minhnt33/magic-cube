using UnityEngine;
using System.Collections;

public class EffectStun : BaseBulletEffect
{
    public float _duration;
    public GameObject _stunEffect;

    public delegate void OnStunEffect(GameObject target, float duration);
    public static event OnStunEffect OnStunEffectEvent;

    public override void TriggerEffect(GameObject targetGO)
    {
        int layer = targetGO.layer;
        if (layer == 8 || layer == 9)
        {
            Vector3 pos = targetGO.transform.position;
            pos.y = pos.y + targetGO.transform.localScale.y + 1;
            PoolingHelper.Instance.InstantiatePrefab(_stunEffect, pos, Quaternion.identity);

            if (OnStunEffectEvent != null)
                OnStunEffectEvent(targetGO, _duration);
        }
    }
}
