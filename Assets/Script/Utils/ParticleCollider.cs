using UnityEngine;
using System.Collections;

public class ParticleCollider : MonoBehaviour
{
    public float _delayTime;
    public Vector3 _size;
    public Vector3 _center;

    private BoxCollider _collider;

    void Awake()
    {
        CreateCollider();
    }

    void Start()
    {
        StartCoroutine(CreateColliderCoroutine());
    }

    void OnDisable()
    {
        _collider.enabled = false;
    }

    IEnumerator CreateColliderCoroutine()
    {
        yield return new WaitForSeconds(_delayTime);

        _collider.enabled = true;
    }

    private void CreateCollider()
    {
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.size = _size;
        _collider.center = _center;
        _collider.isTrigger = true;
        _collider.enabled = false;
    }
}
