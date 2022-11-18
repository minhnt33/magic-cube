using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossFightScore : BaseScore
{

    protected override void OnEnable()
    {
        base.OnEnable();
        ModeBoss.OnTimerChangeEvent += OnTimeChangeEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ModeBoss.OnTimerChangeEvent -= OnTimeChangeEvent;
    }

    public override int CalculateStar()
    {
        if (_score < _starConditions[3])
            return 3;
        else if (_score < _starConditions[2] && _score >= _starConditions[3])
            return 2;

        return 1;
    }

    private void OnTimeChangeEvent(float currentTime)
    {
        _score = currentTime;
    }

    public override bool HighScoreCondition(float currentBest)
    {
        if (currentBest == 0)
            return true;

        else if (_score < currentBest)
            return true;

        return false;
    }
}
