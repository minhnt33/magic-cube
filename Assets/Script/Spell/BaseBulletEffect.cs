using UnityEngine;
using System.Collections;

public abstract class BaseBulletEffect : MonoBehaviour
{
    protected Transform _trans;

    public abstract void TriggerEffect(GameObject targetGO);

    protected virtual void Awake()
    {
        _trans = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if (layer == 8 || layer == 9 || layer == 10 || layer == 11)
        {
            TriggerEffect(other.gameObject);
        }

        PoolingHelper.Instance.Destroy(gameObject);
    }
}
