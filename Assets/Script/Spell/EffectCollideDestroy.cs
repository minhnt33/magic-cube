using UnityEngine;
using System.Collections;

public class EffectCollideDestroy : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        PoolingHelper.Instance.Destroy(gameObject);
    }
}
