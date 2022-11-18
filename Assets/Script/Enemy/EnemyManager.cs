using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;

public class EnemyManager : MonoBehaviour
{

    private static EnemyManager _instance;

    public static EnemyManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<GameObject> _bossList;
    public List<GameObject> BossList { get { return _bossList; } }

    private List<GameObject> _enemyList;
    public List<GameObject> EnemyList { get { return _enemyList; } }

    public bool IsAllBossDie { get { return _bossList.Count == 0; } }

    void OnEnable()
    {
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
        BaseGameMode.OnPauseAtEndEvent += OnPauseGameAtEnd;
    }

    void OnDisable()
    {
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
        BaseGameMode.OnPauseAtEndEvent -= OnPauseGameAtEnd;
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _bossList = new List<GameObject>();
        _enemyList = new List<GameObject>();
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        CreateEnemy(info);
    }

    private void CreateEnemy(LevelInformation levelInfo)
    {
        LoadedEnemy[] enemyInfoArray = levelInfo.Enemies;
        PathManager[] enemyPathManagers = levelInfo.EnemyPaths;
        PathManager[] bossPathManagers = levelInfo.BossPaths;

        for (int i = 0; i < enemyInfoArray.Length; i++)
        {
            for (int k = 0; k < enemyInfoArray[i].Count; k++)
            {
                GameObject enemy = PoolingHelper.Instance.InstantiatePrefab(gameObject, enemyInfoArray[i].Prefab);
                BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
                splineMove spline = baseEnemy.MovementMethod;
                baseEnemy.LoadedEnemyInfo = enemyInfoArray[i];

                if (enemyInfoArray[i].IsBoss)
                {
                    _bossList.Add(enemy);
                    baseEnemy.AllPathManager = bossPathManagers;
                }
                else
                {
                    _enemyList.Add(enemy);
                    baseEnemy.AllPathManager = enemyPathManagers;
                }

                spline.startPoint = Mathf.Clamp(k, 0, enemyInfoArray[i].Count - 1);
            }
        }
    }

    public void RemoveBoss(GameObject boss)
    {
        _bossList.Remove(boss);
    }

    private void OnPauseGameAtEnd()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<BaseEnemy>().StopAllCoroutines();
        }
    }
}
