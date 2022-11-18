using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{

    private Text _timeText;

    void OnEnable()
    {
        ModeTimeAttack.OnTimerChangeEvent += OnTimerAttackChangeEvent;
        ModeNormal.OnTimerChangeEvent += OnTimerChangeEvent;
        ModeSurvival.OnTimerChangeEvent += OnSurTimerChangeEvent;
        ModeBoss.OnTimerChangeEvent += OnBossTimerChangeEvent;
    }

    void OnDisable()
    {
        ModeTimeAttack.OnTimerChangeEvent -= OnTimerAttackChangeEvent;
        ModeNormal.OnTimerChangeEvent -= OnTimerChangeEvent;
        ModeSurvival.OnTimerChangeEvent -= OnSurTimerChangeEvent;
        ModeBoss.OnTimerChangeEvent -= OnBossTimerChangeEvent;
    }

    void Awake()
    {
        _timeText = GetComponent<Text>();
    }

    private void OnTimerAttackChangeEvent(float totalTime, float currentTime)
    {
        _timeText.text = TimeUtils.FloatToTime(currentTime, "#0:00");
    }

    private void OnTimerChangeEvent(float currentTime)
    {
        _timeText.text = TimeUtils.FloatToTime(currentTime, "#0:00");
    }

    private void OnSurTimerChangeEvent(float currentTime)
    {
        _timeText.text = TimeUtils.FloatToTime(currentTime, "#0:00");
    }

    private void OnBossTimerChangeEvent(float currentTime)
    {
        _timeText.text = TimeUtils.FloatToTime(currentTime, "#0:00");
    }
}
