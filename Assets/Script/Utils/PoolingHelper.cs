using UnityEngine;
using System.Collections;

public class PoolingHelper : MonoBehaviour
{
    public GameObject _playerGO;

    private static PoolingHelper _instance = null;

    public static PoolingHelper Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        InstantiatePrefab(_playerGO);
    }

    public GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rot)
    {
        return TrashMan.spawn(prefab, position, rot);
    }

    public GameObject InstantiatePrefab(GameObject parent, GameObject prefab, Vector3 position, Quaternion rot)
    {
        GameObject go = TrashMan.spawn(prefab, position, rot);
        go.transform.parent = parent.transform;
        return go;
    }

    public GameObject InstantiatePrefab(GameObject prefab)
    {
        return TrashMan.spawn(prefab);
    }

    public GameObject InstantiatePrefab(GameObject parent, GameObject prefab)
    {
        GameObject go = TrashMan.spawn(prefab);
        go.transform.parent = parent.transform;
        return go;
    }

    public void Destroy(GameObject go)
    {
        TrashMan.despawn(go);
    }

    public void DestroyAfter(GameObject go, float delay)
    {
        TrashMan.despawnAfterDelay(go, delay);
    }
}
