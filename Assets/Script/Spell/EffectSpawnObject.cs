using UnityEngine;
using System.Collections;

public class EffectSpawnObject : MonoBehaviour
{
    public GameObject _spawnedGO;
    public Vector3 _offsetPosition;
    public float _delaySpawn;

    private Transform _trans;

    void Awake()
    {
        _trans = transform;
    }

    // Use this for initialization
    void Start()
    {
        if (_delaySpawn > 0f)
            StartCoroutine(Delay(_delaySpawn));
        else
            PoolingHelper.Instance.InstantiatePrefab(_spawnedGO, _trans.position + _offsetPosition, Quaternion.identity);
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolingHelper.Instance.InstantiatePrefab(_spawnedGO, _trans.position + _offsetPosition, Quaternion.identity);
    }
}
