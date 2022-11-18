using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIOverMenu : MonoBehaviour
{
    public GameObject _winTextGO;
    public GameObject _loseTextGO;

    public Text _score;
    public Text _bestScore;

    void OnEnable()
    {
        BaseScore.OnGetResultEvent += OnGetResultEvent;
    }

    void OnDisable()
    {
        BaseScore.OnGetResultEvent -= OnGetResultEvent;
    }

    private void OnGetResultEvent(int starNumber, float score, float bestScore)
    {
        if (starNumber == 0)
            _loseTextGO.SetActive(true);
        else
            _winTextGO.SetActive(true);

        _bestScore.text = TimeUtils.FloatToTime(bestScore, "#0:00");
        StartCoroutine(IncreasingPoint(score, 3f));
    }

    IEnumerator IncreasingPoint(float score, float duration)
    {
        float timer = duration;
        float progress = 0f;
        float tmpScore = 0f;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(duration, 0f, timer);
            tmpScore = Mathf.Lerp(0f, score, progress);
            _score.text = TimeUtils.FloatToTime(Mathf.Round(tmpScore), "#0:00");

            if (progress == 1f)
                yield break;

            yield return null;
        }
    }
}
