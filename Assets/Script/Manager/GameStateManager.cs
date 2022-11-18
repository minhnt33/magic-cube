using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager _instance;

    public static GameStateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private GAME_STATE _gameState;
    private GAME_STATE _prevState;

    public delegate void OnGameStateChangeEvent(GAME_STATE newState);
    public static event OnGameStateChangeEvent OnGameStateChange;


    void OnEnable()
    {
        FirstCutSceneCam.OnEndCutSceneEvent += OnEndCutSceneEvent;
    }

    void OnDisable()
    {
        FirstCutSceneCam.OnEndCutSceneEvent -= OnEndCutSceneEvent;
    }

    // Use this for initialization
    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        SetGameState(GAME_STATE.CUT_SCENE);

        PrefManager.Instance.AddGamePlayCount();
    }

    public void SetGameState(GAME_STATE newState)
    {
        _prevState = _gameState;
        _gameState = newState;

        if (OnGameStateChange != null)
            OnGameStateChange(_gameState);
    }

    private void OnEndCutSceneEvent()
    {
        SetGameState(GAME_STATE.PLAYING);
    }

    public GAME_STATE GetGameState()
    {
        return _gameState;
    }

    public GAME_STATE GetPreviousState()
    {
        return _prevState;
    }

    public void SetStateDelay(GAME_STATE newState, float delay)
    {
        StartCoroutine(SetStateDelayCoroutine(newState, delay));
    }

    IEnumerator SetStateDelayCoroutine(GAME_STATE newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetGameState(newState);
    }
}
