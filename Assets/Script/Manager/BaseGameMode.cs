using UnityEngine;
using System.Collections;

public abstract class BaseGameMode
{
    protected GameObject[] _disabledGO;
    protected GameObject[] _enabledGO;

    protected bool _playerDie;
    protected GameObject _playerGO;

    public BaseGameMode(GameObject[] disabled, GameObject[] enabled)
    {
        _disabledGO = disabled;
        _enabledGO = enabled;
        _playerGO = GameObject.FindGameObjectWithTag("Player");
        ActorHealth.OnHealthZero += OnPlayerDie;
        GameStateManager.OnGameStateChange += OnGameStateChange;
    }

    public delegate void OnGameReturnResult(bool playerDie);
    public static event OnGameReturnResult OnGameReturnResultEvent;

    public delegate void OnPauseAtEnd();
    public static event OnPauseAtEnd OnPauseAtEndEvent;

    public abstract IEnumerator CheckingWinConditionCoroutine();

    protected void ReturnGameResult()
    {
        SetGOState(_enabledGO, true);

        if (OnGameReturnResultEvent != null)
            OnGameReturnResultEvent(_playerDie);
    }

    private void SetGOState(GameObject[] gos, bool active)
    {
        for (int i = 0; i < gos.Length; i++)
            if (gos[i] != null)
                gos[i].SetActive(active);
    }

    protected virtual void OnPlayerDie(GameObject go)
    {
        if (_playerGO != go)
            return;

        _playerDie = true;
        PauseGameAtEnd();
        GameStateManager.Instance.SetStateDelay(GAME_STATE.END, 5f);
    }

    protected void PauseGameAtEnd()
    {
        SetGOState(_disabledGO, false);

        if (OnPauseAtEndEvent != null)
            OnPauseAtEndEvent();
    }

    private void OnGameStateChange(GAME_STATE newState)
    {
        if (newState != GAME_STATE.END)
            return;

        ReturnGameResult();
    }
}
