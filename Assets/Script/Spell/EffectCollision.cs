using UnityEngine;
using System.Collections;

public class EffectCollision : BaseBulletEffect
{
    public GameObject _collideEffect;
    public Vector3 _effectScale = Vector3.one;
    public SOUND_KEY _soundKey;

    public override void TriggerEffect(GameObject targetGO)
    {
        Transform effectTrans = PoolingHelper.Instance.InstantiatePrefab(_collideEffect, _trans.position, Quaternion.identity).transform;

        effectTrans.localScale = _effectScale;
        AudioManager.Instance.PlaySound(_soundKey);
    }
}
