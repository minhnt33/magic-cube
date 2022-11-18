using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameDifficulties : System.Object
{
    [SerializeField]
    private Difficulties _difficulty;

    [SerializeField]
    private float _spawmCubePeriodTime;
    public float SpawnCubePeriodTime { get { return _spawmCubePeriodTime; } }
}
