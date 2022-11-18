using UnityEngine;
using System.Collections;

public class FlyingEnemyController : BaseEnemy
{
    private EnemyBehavior _curState;

    private BossWalk _walk;
    private BaseEnemyAttack _attack;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Awake()
    {
        base.Awake();
        _walk = GetComponent<BossWalk>();
        _attack = GetComponent<BaseEnemyAttack>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void OnDeath()
    {
    }

    public override void MakingDecision()
    {
        if (MathUtils.RandomBoolean(0.5f))
        {
            _anim.SetBool("Flying", true);
            ChangePath(1);
        }
        else
        {
            _anim.SetBool("Flying", false);
            ChangePath(0);
        }

        if (MathUtils.RandomBoolean(0.6f))
        {
            _curState = _walk;
        }
        else
        {
            _curState = _attack;
        }

        if (_curState != null)
            _curState.execute();
    }
}
