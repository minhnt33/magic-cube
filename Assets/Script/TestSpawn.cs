using UnityEngine;
using System.Collections;

public class TestSpawn : MonoBehaviour {

    public GameObject _prefab;

    void Start()
    {
        //Instantiate(_prefab, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
        PoolingHelper.Instance.InstantiatePrefab(_prefab, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
    }
}
