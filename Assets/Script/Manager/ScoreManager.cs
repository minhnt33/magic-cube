using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    private BaseScore _baseScore;

    void OnEnable()
    {
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
    }

    void OnDisable()
    {
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        //if(info.GameType == GameType.NORMAL)
            
    }
}
