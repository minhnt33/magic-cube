using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeBoss : BaseGameMode
{
    public delegate void OnTimerChange(float currentTime);
    public static event OnTimerChange OnTimerChangeEvent;

    private EnemyManager _enemyManager;

    public ModeBoss(GameObject[] disable, GameObject[] enable)
        : base(disable, enable)
    {
    }

    public override IEnumerator CheckingWinConditionCoroutine()
    {
        _enemyManager = EnemyManager.Instance;

        float currentTime = 0f;
        while (true)
        {
            currentTime += Time.deltaTime;

            if (OnTimerChangeEvent != null)
                OnTimerChangeEvent(Mathf.Round(currentTime));

            if (_enemyManager.IsAllBossDie)
            {
                PauseGameAtEnd();
                GameStateManager.Instance.SetStateDelay(GAME_STATE.END, 5f);
                yield break;
            }

            yield return null;
        }
    }
}
