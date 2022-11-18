using UnityEngine;
using System.Collections;

public class StabBullet : MonoBehaviour
{
    public float _speed;
    public GameObject _boomEffect;

    private Rigidbody _body;
    private Vector3 _dir;
    private bool _fire = false;

    // Use this for initialization
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void SetDirection(Vector3 dir)
    {
        _dir = dir;
    }

    public void Fire(Vector3 dir)
    {
        SetDirection(dir);
        _body.AddForce(dir * _speed);
    }

    void OnTriggerEnter(Collider other)
    {
        Instantiate(_boomEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
