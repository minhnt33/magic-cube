using UnityEngine;
using System.Collections;

public class EnemyStraightAttack : BaseEnemyAttack
{
    public GameObject[] _bulletPrefab;
    public float _bulletSpeed;
    public int _bulletNum;
    public float _angleStep;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Fire()
    {

        Vector3 pos = _attackPoints[Random.Range(0, _attackPoints.Length)].position;

        GameObject prefab = _bulletPrefab[Random.Range(0, _bulletPrefab.Length)];

        Vector3 centerDir = (_target.position - _trans.position).normalized;

        for (int i = 0; i < _bulletNum; i++)
        {
            Vector3 dir = Quaternion.Euler(0f, -_angleStep * i, 0f) * centerDir;

            StraightMovement bullet = PoolingHelper.Instance.InstantiatePrefab(prefab, pos, Quaternion.identity).GetComponent<StraightMovement>();
            bullet.Move(dir);
        }
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
