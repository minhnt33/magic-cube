using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeNormal : BaseGameMode
{
    private GridMapManager _gridManager;
    private CubeManager _cubeManager;
    private List<GridMember> _gridList;

    public delegate void OnTimerChange(float currentTime);
    public static event OnTimerChange OnTimerChangeEvent;

    public delegate void OnNormalModeResult(bool isCubeClear);
    public static event OnNormalModeResult OnNormalModeresultEvent;

    public ModeNormal(GameObject[] disable, GameObject[] enable)
        : base(disable, enable)
    {
        _gridManager = GridMapManager.Instance;
        _cubeManager = CubeManager.Instance;
        _gridList = _gridManager.UnavailableGridList;
    }

    public override IEnumerator CheckingWinConditionCoroutine()
    {
        float currentTime = 0f;
        while (true)
        {
            currentTime += Time.deltaTime;

            if (OnTimerChangeEvent != null)
                OnTimerChangeEvent(Mathf.Round(currentTime));

            if (CheckingLose(_gridList))
            {
                GameStateManager.Instance.SetStateDelay(GAME_STATE.END, 5f);
                yield break;
            }

            yield return null;
        }
    }

    private bool CheckingLose(List<GridMember> list)
    {
        if (list.Count <= 3)
        {
            if (CubeManager.Instance.TotalCountdownCube > 0)
                return false;
            else
                return true;
        }

        return false;
    }
}
