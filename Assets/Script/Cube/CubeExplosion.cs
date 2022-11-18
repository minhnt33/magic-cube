using UnityEngine;
using System.Collections;
using System;

public class CubeExplosion : MonoBehaviour
{
    [SerializeField]
    private Color[] _countdownStateColors = new Color[3];

    private CubeManager _cubeManager;
    private PoolingHelper _poolingHelper;
    private Transform _trans;
    private CubeInformation _cubeInfo;
    private Coroutine _countdownCor = null;
    private Coroutine _scaleCor = null;
    private Renderer _renderer;
    private Array _directions;
    private bool _isRolling = false;

    public delegate void OnCubeExplosion(GameObject cube);
    public static event OnCubeExplosion OnCubeExplosionEvent;

    void OnEnable()
    {
        CubeManager.OnCubeBlockExplodeEvent += OnCubeBlockExplodeEvent;
        BoxRoller.EndRollEvent += EndRollEvent;
        BoxRoller.StartingRollEvent += StartingRollEvent;

        _trans.localScale = Vector3.one;
    }

    void OnDisable()
    {
        CubeManager.OnCubeBlockExplodeEvent -= OnCubeBlockExplodeEvent;
        BoxRoller.EndRollEvent -= EndRollEvent;
        BoxRoller.StartingRollEvent -= StartingRollEvent;

        if (_scaleCor != null)
            StopCoroutine(_scaleCor);
        if (_countdownCor != null)
            StopCoroutine(_countdownCor);

        _countdownCor = null;
        _scaleCor = null;
        _isRolling = false;
    }

    void Awake()
    {
        _trans = transform;
        _cubeInfo = GetComponent<CubeInformation>();
        _renderer = GetComponent<Renderer>();
        _directions = Enum.GetValues(typeof(Direction));
    }

    void Start()
    {
        _cubeManager = CubeManager.Instance;
        _poolingHelper = PoolingHelper.Instance;
    }

    public void BulletTriggerExplode(GridMember grid)
    {
        if (_scaleCor != null || _countdownCor != null)
            return;

        _cubeManager.AddCubeToCountdownList(gameObject);

        _renderer.material.color = _countdownStateColors[0];

        _scaleCor = StartCoroutine(ScaleSizeCorountine(Vector3.one * 1.1f, 1f));

        _countdownCor = StartCoroutine(CountdownCorountine(grid, CubeManager.Instance.GetAttackInfo(gameObject).BaseDelaySeconds));
    }

    private void OnCubeBlockExplodeEvent(GameObject cube, GridMember grid, int cubeNumber)
    {
        if (cube != gameObject)
            return;

        if (_scaleCor != null || _countdownCor != null)
            return;

        CubeManager.Instance.AddCubeToCountdownList(gameObject);

        _renderer.material.color = _countdownStateColors[0];

        _scaleCor = StartCoroutine(ScaleSizeCorountine(Vector3.one * 1.1f, 1f));

        _countdownCor = StartCoroutine(CountdownCorountine(grid, CubeManager.Instance.GetAttackInfo(gameObject).BaseDelaySeconds * cubeNumber));
    }

    private void Explode()
    {
        if (OnCubeExplosionEvent != null)
            OnCubeExplosionEvent(gameObject);


        CubeAttackInfo info = _cubeManager.GetAttackInfo(gameObject);

        AudioManager.Instance.PlaySound(info.ExplosionSoundKey);

        _poolingHelper.InstantiatePrefab(info.ExplodingEffect, _trans.position, info.ExplodingEffect.transform.localRotation);

        if (info.BulletEffect != null)
        {
            foreach (Direction dir in _directions)
            {
                _poolingHelper.InstantiatePrefab(info.BulletEffect, _trans.position, Quaternion.Euler(0f, (int)dir * 90f, 0f)).GetComponent<BaseBulletMovement>().Move();
            }
        }

        _cubeInfo.IsInCountDownState = false;
        _renderer.material.color = Color.white;
    }

    private IEnumerator CountdownCorountine(GridMember grid, float delay)
    {
        float timer = delay;
        float progress = 0f;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(delay, 0f, timer);

            if (progress == 1)
            {
                if (_isRolling)
                    yield return null;

                Explode();

                PoolingHelper.Instance.Destroy(gameObject);

                yield break;
            }
            else if (progress >= 0.4f && progress <= 0.7f)
                _renderer.material.color = _countdownStateColors[1];
            else if (progress > 0.7f)
                _renderer.material.color = _countdownStateColors[2];

            yield return null;
        }
    }

    private IEnumerator ScaleSizeCorountine(Vector3 targetScale, float delay)
    {
        float timer = delay / 2;
        float progress = 0f;
        Vector3 startingScale = _trans.localScale;
        Vector3 destinationScale = targetScale;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(delay, 0f, timer);

            _trans.localScale = Vector3.Lerp(startingScale, destinationScale, progress);

            if (progress == 1f)
            {
                Vector3 tmp = startingScale;
                startingScale = destinationScale;
                destinationScale = tmp;
                progress = 0f;
                timer = delay / 2;
            }
            yield return null;
        }
    }

    private void EndRollEvent(GameObject go)
    {
        if (go != gameObject)
            return;

        _isRolling = false;
    }

    private void StartingRollEvent(GameObject go)
    {
        if (go != gameObject)
            return;

        _isRolling = true;
    }
}
