using UnityEngine;
using System.Collections;

public class ModeSurvival : BaseGameMode
{
    public delegate void OnTimerChange(float currentTime);
    public static event OnTimerChange OnTimerChangeEvent;

    private GridMapManager _gridMapManager;

    public ModeSurvival(GameObject[] disable, GameObject[] enable)
        : base(disable, enable)
    {
        _gridMapManager = GridMapManager.Instance;
    }

    public override IEnumerator CheckingWinConditionCoroutine()
    {
        float currentTime = 0f;
        while (true)
        {
            currentTime += Time.deltaTime;

            if (OnTimerChangeEvent != null)
                OnTimerChangeEvent(Mathf.Round(currentTime));

            if (_gridMapManager.FullGrid)
            {
                PauseGameAtEnd();
                GameStateManager.Instance.SetStateDelay(GAME_STATE.END, 5f);
                yield break;
            }

            yield return null;
        }
    }
}
