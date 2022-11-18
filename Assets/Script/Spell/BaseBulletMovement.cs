using UnityEngine;
using System.Collections;

public abstract class BaseBulletMovement : MonoBehaviour
{
    protected Rigidbody _body;
    protected Transform _trans;

    protected virtual void OnEnable()
    {
        _body.velocity = Vector3.zero;
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void Awake()
    {
        _trans = transform;
        _body = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
    }

    public abstract void Move(Vector3 direction);
    public abstract void Move();
}
