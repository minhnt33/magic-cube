using UnityEngine;
using System.Collections;
using SWS;

public class BossController : BaseEnemy
{
    private BossWalk _walk;
    private BaseEnemyAttack[] _attacks;
    private BossSpell _spell;
    private ActorHealth _health;

    private EnemyBehavior _curState;

    private float[] chanceArray = { 0.3f, 0.5f, 0.2f };

    protected override void OnEnable()
    {
        base.OnEnable();
        ActorHealth.OnHealthZero += OnHealthZero;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ActorHealth.OnHealthZero -= OnHealthZero;
    }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        _walk = GetComponent<BossWalk>();
        _attacks = GetComponents<BaseEnemyAttack>();
        _spell = GetComponent<BossSpell>();
        _health = GetComponent<ActorHealth>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void MakingDecision()
    {
        //int rand = MathUtils.RandomArrayIndexByChance(chanceArray);
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            _curState = _walk;
        }
        else if (rand == 1 || rand == 2)
        {
            _curState = _attacks[Random.Range(0, _attacks.Length)];
        }
        else
        {
            _curState = _spell;
        }

        if (_curState != null)
            _curState.execute();
    }

    public override void OnDeath()
    {
        _curState = null;
    }

    private void OnHealthZero(GameObject go)
    {
        if (go != gameObject)
            return;

        EnemyManager.Instance.RemoveBoss(gameObject);
    }
}
