using UnityEngine;
using System.Collections;
using MadLevelManager;

public abstract class BaseScore : MonoBehaviour
{
    protected float _score;
    protected int _numberOfStart;
    public int NumberOfStar { get { return _numberOfStart; } }

    public delegate void OnGetResult(int starNumber, float score, float bestScore);
    public static event OnGetResult OnGetResultEvent;

    protected int[] _starConditions;

    protected virtual void OnEnable()
    {
        GameStateManager.OnGameStateChange += OnGameStateChange;
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
        BaseGameMode.OnGameReturnResultEvent += OnGameResultEvent;
    }

    protected virtual void OnDisable()
    {
        GameStateManager.OnGameStateChange -= OnGameStateChange;
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
        BaseGameMode.OnGameReturnResultEvent -= OnGameResultEvent;
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        _starConditions = info.StarLevelConditions;
    }

    public abstract int CalculateStar();
    public abstract bool HighScoreCondition(float currentBest);

    private bool _playerDie;
    private void OnGameResultEvent(bool playerDie)
    {
        _playerDie = playerDie;
    }

    private void OnGameStateChange(GAME_STATE newstate)
    {
        if (newstate != GAME_STATE.END)
            return;

        if (!_playerDie)
            _numberOfStart = CalculateStar();
        else
        {
            _numberOfStart = 0;
            _score = 0;
        }

        float bestScore = MadLevelProfile.GetLevelFloat(MadLevel.currentLevelName, "BestScore");

        if (HighScoreCondition(bestScore))
        {
            MadLevelProfile.SetLevelFloat(MadLevel.currentLevelName, "BestScore", _score);
            bestScore = _score;
        }


        // gain stars
        for (int i = 1; i <= _numberOfStart; i++)
        {
            string starName = "star_" + i;
            MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, starName, true);
        }

        // complete level
        if (_numberOfStart != 0)
        {
            MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
        }

        if (OnGetResultEvent != null)
            OnGetResultEvent(_numberOfStart, _score, bestScore);
    }
}
