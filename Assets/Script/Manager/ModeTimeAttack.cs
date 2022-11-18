using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeTimeAttack : BaseGameMode
{
    public delegate void OnTimerChange(float totalTime, float currentTime);
    public static event OnTimerChange OnTimerChangeEvent;

    private GridMapManager _gridManager;

    public ModeTimeAttack(GameObject[] disable, GameObject[] enable)
        : base(disable, enable)
    {
        _gridManager = GridMapManager.Instance;
    }

    public override IEnumerator CheckingWinConditionCoroutine()
    {
        float totalTime = LevelLoader.Instance.LoadedLevelInfo.TimeAttackLimit;
        float currentTime = totalTime;

        while (true)
        {
            currentTime -= Time.deltaTime;

            if (OnTimerChangeEvent != null)
                OnTimerChangeEvent(totalTime, Mathf.Round(currentTime));

            if (currentTime <= 0f || _gridManager.EmptyGrid)
            {
                PauseGameAtEnd();
                GameStateManager.Instance.SetStateDelay(GAME_STATE.END, 5f);
                yield break;
            }

            yield return null;
        }
    }
}
