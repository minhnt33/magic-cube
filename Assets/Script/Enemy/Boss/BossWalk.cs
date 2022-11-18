using UnityEngine;
using System.Collections;
using SWS;

public class BossWalk : MonoBehaviour, EnemyBehavior {
    
    private splineMove _splineMove;

    void Awake()
    {
        _splineMove = GetComponent<splineMove>();
    }

    public void execute()
    {
        _splineMove.Resume();
    }
}
