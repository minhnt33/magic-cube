using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MagicCubeManager;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public enum CLIPTYPE
    {
        BACKGROUND, TEMPORARY
    };

    public AudioSource _uiAudioSource;
    public AudioClipInfo[] _backgroundAudios;
    public AudioClipInfo[] _tmpAudios;
    public AudioClipInfo[] _gamePlayBg;

    private AudioSource[] _allSources;

    private Dictionary<SOUND_KEY, int> _clipInfoDict;

    void OnEnable()
    {
        SceneManager.OnSceneChange += OnSceneChange;
        GameStateManager.OnGameStateChange += OnGameStateChange;
        BoxRoller.EndRollEvent += OnEndRollEvent;
        BaseScore.OnGetResultEvent += OnGetResult;
    }

    void OnDisable()
    {
        SceneManager.OnSceneChange -= OnSceneChange;
        GameStateManager.OnGameStateChange -= OnGameStateChange;
        BoxRoller.EndRollEvent -= OnEndRollEvent;
        BaseScore.OnGetResultEvent -= OnGetResult;
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        PreProcessingAudioArray();
    }

    private void PreProcessingAudioArray()
    {
        _clipInfoDict = new Dictionary<SOUND_KEY, int>();
        bool audioState = PrefManager.Instance.GetSoundSetting();

        _allSources = GetComponentsInChildren<AudioSource>(true);

        if (audioState)
            SetAudiosState(false);
        else
            SetAudiosState(true);

        for (int i = 0; i < _backgroundAudios.Length; i++)
        {
            _clipInfoDict.Add(_backgroundAudios[i].uniqueKey, i);
        }

        for (int i = 0; i < _tmpAudios.Length; i++)
        {
            _clipInfoDict.Add(_tmpAudios[i].uniqueKey, i);
        }
    }

    public void SetAudiosState(bool isMute)
    {
        for (int i = 0; i < _allSources.Length; i++)
            _allSources[i].mute = isMute;
    }

    #region PLAY AUDIO
    public AudioSource PlayLoop(CLIPTYPE type, SOUND_KEY key)
    {

        AudioClipInfo clipInfo = GetAudioClipInfo(type, key);

        if (clipInfo == null)
            return null;

        clipInfo.audioSource.Stop();
        clipInfo.audioSource.clip = clipInfo.audioClip;
        clipInfo.audioSource.Play();

        return clipInfo.audioSource;
    }

    public AudioSource PlayOne(CLIPTYPE type, SOUND_KEY key)
    {
        AudioClipInfo clipInfo = GetAudioClipInfo(type, key);

        if (clipInfo == null)
            return null;

        clipInfo.audioSource.Stop();
        clipInfo.audioSource.PlayOneShot(clipInfo.audioClip);
        return clipInfo.audioSource;
    }

    public void PlayOne(AudioClip clip)
    {
        _uiAudioSource.PlayOneShot(clip);
    }

    //public void PlayOne(AudioClip clip)
    //{
    //    _uiAudioSource.PlayOneShot(clip, PrefManager.Instance.GetSetting().soundVol);
    //}

    public AudioSource PlayOneCustomSource(CLIPTYPE type, SOUND_KEY key, AudioSource customSource)
    {

        AudioClipInfo clipInfo = GetAudioClipInfo(type, key);

        if (clipInfo == null)
            return null;

        clipInfo.audioSource = customSource;
        clipInfo.audioSource.Stop();
        clipInfo.audioSource.PlayOneShot(clipInfo.audioClip);
        return clipInfo.audioSource;
    }

    public AudioSource PlayMusic(SOUND_KEY key)
    {
        return PlayLoop(CLIPTYPE.BACKGROUND, key);
    }

    public AudioSource PlaySound(SOUND_KEY key)
    {
        return PlayOne(CLIPTYPE.TEMPORARY, key);
    }

    public void PlaySoundDelay(SOUND_KEY key, float delay)
    {
        StartCoroutine(PlaySoundDelayCoroutine(key, delay));
    }

    public AudioSource PlayRandomMusic(AudioClipInfo[] clipInfoArray)
    {
        AudioClipInfo clipInfo = clipInfoArray[Random.Range(0, clipInfoArray.Length)];

        clipInfo.audioSource.Stop();
        clipInfo.audioSource.clip = clipInfo.audioClip;
        clipInfo.audioSource.Play();
        return clipInfo.audioSource;
    }
    #endregion

    #region STOP AUDIO
    public void StopClip(CLIPTYPE type, SOUND_KEY key)
    {

        AudioClipInfo clipInfo = GetAudioClipInfo(type, key);

        if (clipInfo == null)
            return;

        clipInfo.audioSource.Stop();
        clipInfo.audioSource.clip = null;
    }

    public void StopAllSound()
    {
        for (int i = 0; i < _backgroundAudios.Length; i++)
        {
            _backgroundAudios[i].audioSource.Stop();
        }

        for (int i = 0; i < _tmpAudios.Length; i++)
        {
            _tmpAudios[i].audioSource.Stop();
        }
    }

    public void PauseAllSound()
    {
        for (int i = 0; i < _backgroundAudios.Length; i++)
        {
            _backgroundAudios[i].audioSource.Pause();
        }

        for (int i = 0; i < _tmpAudios.Length; i++)
        {
            _tmpAudios[i].audioSource.Pause();
        }
    }

    public void UnpauseAllSound()
    {
        for (int i = 0; i < _backgroundAudios.Length; i++)
        {
            _backgroundAudios[i].audioSource.UnPause();
        }

        for (int i = 0; i < _tmpAudios.Length; i++)
        {
            _tmpAudios[i].audioSource.UnPause();
        }
    }
    # endregion

    # region GET METHODE
    private AudioClipInfo GetAudioClipInfo(CLIPTYPE type, SOUND_KEY key)
    {
        AudioClipInfo[] clipArray = type == CLIPTYPE.BACKGROUND ? _backgroundAudios : _tmpAudios;

        if (!_clipInfoDict.ContainsKey(key))
            return null;

        int index = _clipInfoDict[key];

        return clipArray[index];
    }

    public AudioClipInfo GetRandomBackgroundClipInfo()
    {
        return _backgroundAudios[Random.Range(0, _backgroundAudios.Length)];
    }

    public AudioClipInfo GetRandomTemporaryClipInfo()
    {
        return _tmpAudios[Random.Range(0, _tmpAudios.Length)];
    }
    # endregion


    public void OnSaveSetting(SettingProperties prop)
    {
        for (int i = 0; i < _backgroundAudios.Length; i++)
        {
            _backgroundAudios[i].audioSource.volume = prop.musicVol;
        }

        for (int i = 0; i < _tmpAudios.Length; i++)
        {
            _tmpAudios[i].audioSource.volume = prop.soundVol;
        }
    }

    #region EVENT LISTENER
    private void OnSceneChange(SceneState scene)
    {
        if (scene == SceneState.MENU_SCENE)
            PlayMusic(SOUND_KEY.MENU_BG);
        else if (scene == SceneState.GAME_SCENE)
            PlayRandomMusic(_gamePlayBg);
    }

    private void OnGameStateChange(GAME_STATE newGameState)
    {
        if (newGameState == GAME_STATE.CUT_SCENE)
            PlaySoundDelay(SOUND_KEY.READY, 3f);
    }

    private void OnEndRollEvent(GameObject go)
    {
        PlaySound(SOUND_KEY.CUBE_ROLL);
    }

    private void OnGetResult(int starNumber, float score, float bestScore)
    {
        if (starNumber == 0)
            PlaySound(SOUND_KEY.SOUND_FAIL);
        else
            PlaySound(SOUND_KEY.SOUND_WIN);
    }

    #endregion

    IEnumerator PlaySoundDelayCoroutine(SOUND_KEY key, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(key);
        yield break;
    }
}
