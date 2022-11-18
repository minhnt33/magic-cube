using UnityEngine;
using System.Collections;

public class EffectSlow : BaseBulletEffect
{
    [SerializeField]
    private float _slowPercentage;
    [SerializeField]
    private float _slowDuration;

    private PlayerMovement _playerMovement;

    public delegate void OnSlowEffect(GameObject targetGO, float duration);
    public static event OnSlowEffect OnSlowEffectEvent;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    public override void TriggerEffect(GameObject targetGO)
    {
        if (OnSlowEffectEvent != null)
            OnSlowEffectEvent(targetGO, _slowDuration);

        if (targetGO.layer == 8)
            _playerMovement.SlowSpeed(_slowPercentage, _slowDuration);
        else if (targetGO.layer == 9)
            targetGO.GetComponent<BaseEnemy>().SlowMovement(_slowPercentage, _slowDuration);
    }
}
