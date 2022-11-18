using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;
using System;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField]
    protected SOUND_KEY _startingSound;

    [SerializeField]
    private float _decisionPeriod;

    [SerializeField]
    private float _disappearDis;

    [SerializeField]
    private float _disappearDuration;

    protected splineMove _splineMove;
    public splineMove MovementMethod { get { return _splineMove; } }
    public PathManager[] AllPathManager { get; set; }
    public LoadedEnemy LoadedEnemyInfo { set; get; }

    protected Animator _anim;
    protected Transform _trans;
    private bool _isDie = false;
    private WaitForSeconds _wait;
    private Coroutine _decisionCor;
    private Coroutine _slowCor;
    private int[] _pathIndexs;

    private int _currentPathIndex;

    protected virtual void OnEnable()
    {
        _isDie = false;
        ActorHealth.OnHealthZero += OnHealthZero;
        FirstCutSceneCam.OnEndCutSceneEvent += OnCutSceneEnd;
        GameStateManager.OnGameStateChange += OnGameStateChange;
        EffectStun.OnStunEffectEvent += OnStunEvent;
    }

    protected virtual void OnDisable()
    {
        ActorHealth.OnHealthZero -= OnHealthZero;
        FirstCutSceneCam.OnEndCutSceneEvent -= OnCutSceneEnd;
        GameStateManager.OnGameStateChange -= OnGameStateChange;
        EffectStun.OnStunEffectEvent -= OnStunEvent;
    }

    protected virtual void Awake()
    {
        _trans = transform;
        _wait = new WaitForSeconds(_decisionPeriod);
        _splineMove = GetComponent<splineMove>();
        _anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        _pathIndexs = LoadedEnemyInfo.PathIndexs;
        AudioManager.Instance.PlaySound(_startingSound);
        _currentPathIndex = 0;
        _splineMove.startPoint = 0;
        _splineMove.SetPath(GetPath(_pathIndexs[_currentPathIndex]));
        //_trans.position = _splineMove.pathContainer.waypoints[_splineMove.startPoint].position;
        _splineMove.StartMove();
    }

    public abstract void OnDeath();
    public abstract void MakingDecision();

    protected void ChangeToNextPath()
    {
        _currentPathIndex = Mathf.Clamp(_currentPathIndex + 1, 0, _pathIndexs.Length - 1);
        ChangePath(_currentPathIndex);
    }

    protected void ChangePath(int index)
    {
        if (index == _currentPathIndex)
            return;

        _splineMove.startPoint = _splineMove.NextPoint;
        _splineMove.moveToPath = true;
        _splineMove.SetPath(AllPathManager[_pathIndexs[index]]);
        _currentPathIndex = index;
    }

    protected PathManager GetPath(int index)
    {
        return AllPathManager[index];
    }

    private void OnCutSceneEnd()
    {
        _splineMove.Resume();
        _decisionCor = StartCoroutine(MakeDecision());
    }

    public void SlowMovement(float percent, float duration)
    {
        if (_slowCor != null)
            return;

        _slowCor = StartCoroutine(SlowCoroutine(percent, duration));
    }

    private IEnumerator SlowCoroutine(float percent, float duration)
    {
        float originSpeed = _splineMove.speed;
        _splineMove.speed = _splineMove.speed * percent;
        yield return new WaitForSeconds(duration);
        _splineMove.speed = originSpeed;
        _slowCor = null;
        yield break;
    }

    private void OnStunEvent(GameObject target, float duration)
    {
        if (target != gameObject)
            return;

        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        _splineMove.Pause();
        yield return new WaitForSeconds(duration);
        _splineMove.Resume();
        yield break;
    }

    IEnumerator MakeDecision()
    {
        while (true)
        {
            MakingDecision();
            yield return _wait;
        }
    }

    private void OnHealthZero(GameObject go)
    {
        if (go != gameObject)
            return;

        _isDie = true;
        _splineMove.Stop();
        _anim.SetTrigger("Die");
        StartCoroutine(DisappearCoroutine(_disappearDis, _disappearDuration));
        OnDeath();
    }

    private void OnGameStateChange(GAME_STATE newState)
    {
        if (newState == GAME_STATE.END && _isDie)
        {
            _splineMove.Stop();

            if (_decisionCor != null)
                StopCoroutine(_decisionCor);
        }
        else if (newState == GAME_STATE.END && !_isDie)
        {
            _splineMove.Stop();
            _anim.SetTrigger("Idle");

            if (_decisionCor != null)
                StopCoroutine(_decisionCor);
        }
    }

    IEnumerator DisappearCoroutine(float dis, float durataion)
    {
        float timer = durataion;
        float progress = 0f;
        Vector3 startingPos = _trans.position;
        Vector3 desPos = startingPos;
        desPos.y = desPos.y - dis;
        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(durataion, 0f, timer);

            _trans.position = Vector3.Lerp(startingPos, desPos, progress);

            if (progress == 1f)
            {
                PoolingHelper.Instance.Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
