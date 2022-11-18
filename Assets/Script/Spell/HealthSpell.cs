using UnityEngine;
using System.Collections;

public class HealthSpell : BaseBuffSpell
{

    [SerializeField]
    private float _healthAmount;

    public delegate void OnHealthBuff(float amount);
    public static event OnHealthBuff OnHealthBuffEvent;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Buff()
    {
        if (OnHealthBuffEvent != null)
            OnHealthBuffEvent(_healthAmount);
    }

}
