using UnityEngine;
using System.Collections;
using MagicCubeManager;

public class PrefManager : MonoBehaviour
{
    private static PrefManager _instance;

    public static PrefManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private const string _gameOpenCountPref = "gameOpenCount";
    private const string _gamePlayCountPref = "gamePlayCount";
    private const string _soundOn = "soundOn";

    public int GameplayInCurrentSession { set; get; }

    public int GameplayCount { get { return PlayerPrefs.GetInt(_gamePlayCountPref); } }
    public int GameOpenCount { get { return PlayerPrefs.GetInt(_gameOpenCountPref); } }
    public bool IsFirstOpeningGame { get { return GameOpenCount == 1 && GameplayCount == 0; } }
    public bool IsFirstPlayingGame { get { return GameOpenCount == 1 && GameplayCount == 1; } }

    void OnEnable()
    {
        SceneManager.OnSceneChange += OnSceneChange;
    }

    void OnDisable()
    {
        SceneManager.OnSceneChange -= OnSceneChange;
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        AddGameOpenCount();
    }

    private void OnSceneChange(SceneState newScene)
    {
        if (newScene == SceneState.GAME_SCENE)
        {
            AddGamePlayCount();
            GameplayInCurrentSession++;
        }
    }

    public void AddGamePlayCount()
    {
        int gamePlayCount = PlayerPrefs.GetInt(_gamePlayCountPref);
        PlayerPrefs.SetInt(_gamePlayCountPref, gamePlayCount + 1);
        PlayerPrefs.Save();
    }

    public int GetGamePlayCount()
    {
        return PlayerPrefs.GetInt(_gamePlayCountPref);
    }

    public void ResetGamePlayCount()
    {
        PlayerPrefs.SetInt(_gamePlayCountPref, 0);
    }

    public void AddGameOpenCount()
    {
        int gameOpenCount = PlayerPrefs.GetInt(_gameOpenCountPref);
        PlayerPrefs.SetInt(_gameOpenCountPref, gameOpenCount + 1);
        PlayerPrefs.Save();
    }

    public void SaveSoundSetting(bool soundState)
    {
        PlayerPrefs.SetInt(_soundOn, soundState ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool GetSoundSetting()
    {
        return PlayerPrefs.GetInt(_soundOn, 1) == 1 ? true : false;
    }
}
