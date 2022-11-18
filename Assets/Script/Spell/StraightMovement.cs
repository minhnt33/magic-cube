using UnityEngine;
using System.Collections;

public class StraightMovement : BaseBulletMovement
{
    [SerializeField]
    private float _initialSpeed;
    public float InitialSpeed { set { _initialSpeed = value; } get { return _initialSpeed; } }
    public Vector3 _torque;

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
    }

    public override void Move()
    {
        _body.AddTorque(_torque, ForceMode.Impulse);
        _body.AddForce(_trans.forward * _initialSpeed, ForceMode.Impulse);
    }

    public override void Move(Vector3 direction)
    {
        _body.AddTorque(_torque, ForceMode.Impulse);
        _body.AddForce(direction * _initialSpeed, ForceMode.Impulse);
    }
}
