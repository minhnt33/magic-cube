using UnityEngine;
using System.Collections;

public class GameModeManager : MonoBehaviour
{
    private static GameModeManager _instance;

    public static GameModeManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private GameType _gameType;
    [SerializeField]
    private GameObject[] _disabledArray;
    [SerializeField]
    private GameObject[] _enabledArray;

    private BaseGameMode _currentGameMode;
    private BaseScore _currentScoreManager;
    public BaseScore ScoreManager { get { return _currentScoreManager; } }

    private Coroutine _checkingCor;

    public delegate void OnSetGameMode(GameType mode);
    public static event OnSetGameMode OnSetGameModeEvent;

    void OnEnable()
    {
        FirstCutSceneCam.OnEndCutSceneEvent += OnEndCutSceneEvent;
        GameStateManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDisable()
    {
        FirstCutSceneCam.OnEndCutSceneEvent -= OnEndCutSceneEvent;
        GameStateManager.OnGameStateChange -= OnGameStateChange;
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _gameType = LevelLoader.Instance.LoadedLevelInfo.GameType;

        if (_gameType == GameType.NORMAL)
        {
            _currentGameMode = new ModeNormal(_disabledArray, _enabledArray);
            _currentScoreManager = gameObject.AddComponent<NormalScore>();
        }
        else if (_gameType == GameType.TIME_ATTACK)
        {
            _currentGameMode = new ModeTimeAttack(_disabledArray, _enabledArray);
            _currentScoreManager = gameObject.AddComponent<TimeAttackScore>();
        }
        else if (_gameType == GameType.BOSS_FIGHT)
        {
            _currentGameMode = new ModeBoss(_disabledArray, _enabledArray);
            _currentScoreManager = gameObject.AddComponent<BossFightScore>();
        }
        else if (_gameType == GameType.SURVIVAL)
        {
            _currentGameMode = new ModeSurvival(_disabledArray, _enabledArray);
            _currentScoreManager = gameObject.AddComponent<SurvivalScore>();
        }
    }

    void Start()
    {
        if (OnSetGameModeEvent != null)
            OnSetGameModeEvent(_gameType);
    }

    private void OnEndCutSceneEvent()
    {
        _checkingCor = StartCoroutine(_currentGameMode.CheckingWinConditionCoroutine());
    }

    private void OnGameStateChange(GAME_STATE gameState)
    {
        if (gameState == GAME_STATE.END)
            StopCoroutine(_checkingCor);
    }
}