﻿using UnityEngine;
using System.Collections;

public class SurvivalScore : BaseScore
{

    protected override void OnEnable()
    {
        base.OnEnable();
        ModeSurvival.OnTimerChangeEvent += OnTimeChangeEvent;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ModeSurvival.OnTimerChangeEvent -= OnTimeChangeEvent;
    }

    private void OnTimeChangeEvent(float currentTime)
    {
        _score = currentTime;
    }

    public override int CalculateStar()
    {
        if (_score <= _starConditions[0])
            return 0;
        else if (_score >= _starConditions[0] && _score < _starConditions[1])
            return 1;
        else if (_score >= _starConditions[1] && _score < _starConditions[2])
            return 2;

        return 3;
    }

    public override bool HighScoreCondition(float currentBest)
    {
        if (_score > currentBest)
            return true;

        return false;
    }
}