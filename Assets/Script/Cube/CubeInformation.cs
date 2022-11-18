using UnityEngine;
using System.Collections;

public class CubeInformation : MonoBehaviour
{
    [SerializeField]
    private CubeType _cubeType;
    public CubeType GetCubeType { get { return _cubeType; } }

    [SerializeField]
    private bool _isCountdownState;
    public bool IsInCountDownState { set { _isCountdownState = value; } get { return _isCountdownState; } }

    [SerializeField]
    private bool _isInSpawningState;
    public bool IsInSpawningState { get { return _isInSpawningState; } }

    [SerializeField]
    private CubeSkill _currentSkill;
    public CubeSkill CurrentSkill { set { _currentSkill = value; } get { return _currentSkill; } }

    [SerializeField]
    private CubeFace _currentFace;
    public CubeFace CurrentFace
    {
        set
        {
            _currentFace = value;
            _currentSkill = CubeFaceManager.Instance.GetSkillByFace(_currentFace);
        }
        get { return _currentFace; }
    }

    [SerializeField]
    private Index _currentIndex;
    public Index CurrentIndex { set { _currentIndex = value; } get { return _currentIndex; } }

    private GridMember _currentGrid;
    public GridMember CurrentGrid { set { _currentGrid = value; } get { return _currentGrid; } }

    private Transform _trans;
    private GridMapManager _gridManager;
    private CubeManager _cubeManager;

    void OnEnable()
    {
        BoxRoller.EndRollEvent += EndRollEvent;
        PlayerCubeControl.OnCubeFollowEvent += OnCubeFollowEvent;
        CubeGenerator.OnCubeSpawnCompletelyEvent += OnCubeSpawnCompletelyEvent;
        CubeGenerator.OnCubeSpawnProgressEvent += OnCubeSpawnProgressEvent;

        if (_cubeType == CubeType.MAGIC)
            CubeExplosion.OnCubeExplosionEvent += OnCubeExplosionEvent;

        // Update grid info when enable
        GameObject grid = RaycastUtils.GetGrid(_trans.position);
        if (grid)
            _currentGrid = grid.GetComponent<GridMember>();
        if (_currentGrid)
            _currentIndex = _currentGrid.GridIndex;
    }

    void OnDisable()
    {
        BoxRoller.EndRollEvent -= EndRollEvent;
        PlayerCubeControl.OnCubeFollowEvent -= OnCubeFollowEvent;
        CubeGenerator.OnCubeSpawnCompletelyEvent -=
            OnCubeSpawnCompletelyEvent;
        CubeGenerator.OnCubeSpawnProgressEvent -= OnCubeSpawnProgressEvent;

        if (_cubeType == CubeType.MAGIC)
        {
            CubeExplosion.OnCubeExplosionEvent -= OnCubeExplosionEvent;
        }
    }

    void Awake()
    {
        _trans = transform;
    }

    void Start()
    {
        _gridManager = GridMapManager.Instance;
        _cubeManager = CubeManager.Instance;
    }

    private void EndRollEvent(GameObject go)
    {
        if (go != gameObject)
            return;

        GameObject gridGO = RaycastUtils.GetGrid(_trans.position);

        if (gridGO)
            _currentGrid = gridGO.GetComponent<GridMember>();

        if (_currentGrid)
            _currentIndex = _currentGrid.GridIndex;

        if (_cubeType == CubeType.MAGIC)
        {
            _currentFace = CubeFaceManager.Instance.UpdateCurrentShowingFace(_trans);
            _currentSkill = CubeFaceManager.Instance.GetSkillByFace(_currentFace);
            _cubeManager.DeleteAdjacentCubes(_currentIndex.Row, _currentIndex.Column);
        }
    }

    private void OnCubeFollowEvent(GameObject cube, Direction dir)
    {
        if (cube != gameObject)
            return;

        _gridManager.FreeGrid(_currentGrid);
        _gridManager.UpdateCubeReferenceInGrid(gameObject, _currentIndex, dir);
    }

    private void OnCubeExplosionEvent(GameObject cube)
    {
        if (cube != gameObject)
            return;

        _gridManager.FreeGrid(_currentGrid);
    }

    private void OnCubeSpawnCompletelyEvent(GameObject cube)
    {
        if (cube != gameObject)
            return;

        _isInSpawningState = false;
    }

    private void OnCubeSpawnProgressEvent(GameObject cube, float progress)
    {
        if (cube != gameObject)
            return;

        if (progress == 0)
        {
            _isInSpawningState = true;
        }
    }
}