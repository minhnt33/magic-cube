using UnityEngine;
using System.Collections;

public class TimeAttackScore : BaseScore
{
    private bool _failded = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        ModeTimeAttack.OnTimerChangeEvent += OnTimeChangeEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ModeTimeAttack.OnTimerChangeEvent -= OnTimeChangeEvent;
    }

    private void OnTimeChangeEvent(float totalTime, float currentTime)
    {
        _score = currentTime;
    }

    public override int CalculateStar()
    {
        if (GridMapManager.Instance.EmptyGrid)
        {
            if (_score >= _starConditions[3])
                return 3;
            else if (_score >= _starConditions[2] && _score < _starConditions[3])
                return 2;
            else if (_score >= _starConditions[1] && _score < _starConditions[2])
                return 1;
        }

        return 0;
    }

    public override bool HighScoreCondition(float currentBest)
    {
        if (_score > currentBest)
            return true;

        return false;
    }
}
