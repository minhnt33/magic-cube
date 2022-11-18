using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using SWS;

public class FirstCutSceneCam : MonoBehaviour
{
    [SerializeField]
    private Camera _mainCam;
    [SerializeField]
    private int _pointNumber = 10;
    [SerializeField]
    private float _radius = 10f;
    [SerializeField]
    private GameObject[] _enabledGO;
    [SerializeField]
    private GameObject[] _disabledGO;

    private Transform _trans;
    private Transform _playerTrans;
    private splineMove _splineMove;
    private float _cameraSize;

    public delegate void OnEndCutScene();
    public static event OnEndCutScene OnEndCutSceneEvent;

    void OnEnable()
    {
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
    }

    void OnDisable()
    {
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        _cameraSize = info.CameraSize;
    }

    void Awake()
    {
        _trans = transform;
        _splineMove = GetComponent<splineMove>();
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;


        Vector3 center = _playerTrans.position;
        center.y = _playerTrans.position.y + 6f;

        float sliceAngle = 180 / _pointNumber;

        GameObject wayPoint = new GameObject("WayPoint");
        wayPoint.transform.parent = _trans.parent;

        PathManager pathManager = wayPoint.AddComponent<PathManager>();

        pathManager.waypoints = new Transform[_pointNumber + 1];


        for (int i = 0; i < _pointNumber; i++)
        {
            float angle = i * sliceAngle * Mathf.Deg2Rad;
            float angleY = i * 180 / _pointNumber * Mathf.Deg2Rad;

            Vector3 point = new Vector3((center.x + _radius) * Mathf.Cos(angle), center.y, (center.z + _radius) * Mathf.Cos(angle));

            GameObject pointGO = new GameObject("Point" + i);
            pointGO.transform.position = point;
            pointGO.transform.parent = wayPoint.transform;

            pathManager.waypoints[i] = pointGO.transform;
        }

        pathManager.waypoints[_pointNumber] = _mainCam.transform;

        _trans = pathManager.waypoints[0];
        _splineMove.pathContainer = pathManager;
        _splineMove._lookAtTarget = _playerTrans.position;
    }

    void Start()
    {
        _splineMove.StartMove();
        StartCoroutine(Delay());
    }

    public void OnCameraEndMovement()
    {
        for (int i = 0; i < _enabledGO.Length; i++)
        {
            if (_enabledGO[i] != null)
                _enabledGO[i].SetActive(true);
        }

        for (int k = 0; k < _disabledGO.Length; k++)
        {
            if (_disabledGO[k] != null)
                _disabledGO[k].SetActive(false);
        }

        Camera.main.orthographicSize = _cameraSize;

        if (OnEndCutSceneEvent != null)
            OnEndCutSceneEvent();

        Destroy(_trans.root.gameObject);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(_splineMove.speed);

        OnCameraEndMovement();
    }
}
