using UnityEngine;
using System.Collections;

public class EnemyTarget : MonoBehaviour
{

    [SerializeField]
    private float _seekingPeriod;

    [SerializeField]
    private float _rotatingTime;

    private WaitForSeconds _waitFor;

    private GridMember _targetGrid;
    private Transform _trans;
    private Coroutine _seekingCor;
    private Coroutine _rotatingCor;

    public delegate void OnRotateToTargetComplete(GameObject go);
    public static event OnRotateToTargetComplete OnRotateToTargetCompleteEvent;

    void Awake()
    {
        _trans = transform;
        _waitFor = new WaitForSeconds(_seekingPeriod);
    }

    void Start()
    {
        StartSeekingTarget();
    }

    private void StartSeekingTarget()
    {
        if (_seekingCor != null)
            return;

        _seekingCor = StartCoroutine(SeekingCoroutine());
    }

    private void StartRotatingToTarget()
    {
        if (_rotatingCor != null)
            return;

        _rotatingCor = StartCoroutine(RotateToTargetCorountine(_rotatingTime));
    }

    IEnumerator SeekingCoroutine()
    {
        _targetGrid = GridMapManager.Instance.RandomGrid;
        StartRotatingToTarget();
        yield return _waitFor;
    }

    IEnumerator RotateToTargetCorountine(float secs)
    {
        Vector3 direction = _targetGrid.transform.position - _trans.position;

        Quaternion startingQuat = _trans.rotation;
        Quaternion targetQuat = Quaternion.LookRotation(direction);

        float timer = secs;
        float progress = 0f;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(secs, 0f, timer);

            _trans.rotation = Quaternion.Slerp(startingQuat, targetQuat, progress);

            if (progress == 1f)
            {
                if (OnRotateToTargetCompleteEvent != null)
                    OnRotateToTargetCompleteEvent(gameObject);

                _rotatingCor = null;
                yield break;
            }

            yield return null;
        }
    }

}
