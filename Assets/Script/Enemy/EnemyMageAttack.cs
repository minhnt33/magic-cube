using UnityEngine;
using System.Collections;

public class EnemyMageAttack : BaseEnemyAttack
{
    [SerializeField]
    private GameObject _skillBullet;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Fire()
    {

        Vector3 pos = _attackPoints[Random.Range(0, _attackPoints.Length)].position;

        GameObject bullet = PoolingHelper.Instance.InstantiatePrefab(_skillBullet, pos, _trans.rotation) as GameObject;
        AudioManager.Instance.PlaySound(SOUND_KEY.SKILL_LIGHTNING);
    }

    public void CastSkill()
    {
        Fire();
    }

    public override void SetTransformTarget()
    {
        _target = GridMapManager.Instance.RandomGrid.transform;
        
    }
}
