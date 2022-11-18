using UnityEngine;
using System.Collections;

public class EnemyParabolAttack : BaseEnemyAttack
{
    public GameObject[] _bulletPrefab;
    public Vector3 _bulletScale = Vector3.one;
    public int _bulletNum;

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
            GameObject bullet = PoolingHelper.Instance.InstantiatePrefab(prefab, pos, Quaternion.identity);
            bullet.GetComponent<ParabolicBullet>().Move();
            bullet.transform.localScale = _bulletScale;
        }
    }

    public void CastSkill()
    {
        Fire();
    }

    public override void SetTransformTarget()
    {
        _target = GridMapManager.Instance.RandomUnavailableGrid.transform;
    }
}
