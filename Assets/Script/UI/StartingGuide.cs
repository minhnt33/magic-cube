using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartingGuide : MonoBehaviour
{
    public GameObject _readyTextGO;
    public Sprite _bossFight;
    public Sprite _survival;
    public Sprite _timeAttack;
    public float _displayTime;
    private Image _image;
    private Dictionary<GameType, Sprite> _guideDict;

    void OnEnable()
    {
        GameModeManager.OnSetGameModeEvent += OnSetGameModeEvent;
    }

    void OnDisable()
    {
        GameModeManager.OnSetGameModeEvent -= OnSetGameModeEvent;
    }

    // Use this for initialization
    void Awake()
    {
        _image = GetComponent<Image>();

        _guideDict = new Dictionary<GameType, Sprite>();
        _guideDict.Add(GameType.BOSS_FIGHT, _bossFight);
        _guideDict.Add(GameType.SURVIVAL, _survival);
        _guideDict.Add(GameType.TIME_ATTACK, _timeAttack);
    }

    private void OnSetGameModeEvent(GameType mode)
    {
        _image.sprite = _guideDict[mode];
        StartCoroutine(Delay(_displayTime));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _image.enabled = false;
        _readyTextGO.SetActive(true);
    }
}
