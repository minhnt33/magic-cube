using UnityEngine;
using System.Collections;

public class ParticleDestroyer : MonoBehaviour
{
    [SerializeField]
    private float _delayDestroyTime;

    void Start()
    {
        PoolingHelper.Instance.DestroyAfter(gameObject, _delayDestroyTime);
    }
}
