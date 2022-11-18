using UnityEngine;
using System.Collections;

public class PlayerCubeControl : MonoBehaviour
{
    private Transform _trans;
    private Coroutine _findCubeCor;
    [SerializeField]
    private GameObject _currentCube;
    [SerializeField]
    private Index _currentGridIndex;

    public GameObject CurrentCube
    {
        get { return _currentCube; }
    }

    public Index CurrentGridIndex
    {
        get { return _currentGridIndex; }
    }

    public CubeInformation CurrentCubeInformation { set; get; }

    public delegate void OnCubeFollow(GameObject curCube, Direction dir);
    public static event OnCubeFollow OnCubeFollowEvent;

    public delegate void PlayerOnCube(GameObject cube, bool isDangerous);
    public static event PlayerOnCube PlayerOnCubeEvent;

    public delegate void PlayerOnNoCube();
    public static event PlayerOnNoCube PlayerOnNoCubeEvent;

    private bool _isMoving;

    void OnEnable()
    {
        PlayerMovement.OnPlayerMoveSuccessEvent += OnPlayerMoveSuccessEvent;
        PlayerMovement.OnPlayerEndMoveEvent += OnPlayerEndMoveEvent;
        PlayerMovement.OnPlayerEndJumpCubeEvent += OnPlayerEndJumpCubeEvent;
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
        CubeExplosion.OnCubeExplosionEvent += OnCubeExplosionEvent;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerMoveSuccessEvent -= OnPlayerMoveSuccessEvent;
        PlayerMovement.OnPlayerEndMoveEvent -= OnPlayerEndMoveEvent;
        PlayerMovement.OnPlayerEndJumpCubeEvent -= OnPlayerEndJumpCubeEvent;
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
        CubeExplosion.OnCubeExplosionEvent -= OnCubeExplosionEvent;
    }

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        _trans = transform;
        _currentGridIndex = info.PlayerInitPosition;
        GridMember startingGrid = GridMapManager.Instance.GetGridAt(_currentGridIndex);
        Vector3 pos = startingGrid.Waypoint;
        pos.y = 1.5f;
        _trans.position = pos;
    }

    public void RefreshControlInformation()
    {
        _currentCube = null;
        _currentGridIndex = null;
        CurrentCubeInformation = null;
    }

    private void OnPlayerMoveSuccessEvent(Direction dir)
    {
        _isMoving = true;
        UpdateCubeReferences();

        if (PlayerController.Instance.OnGround)
            return;


        if (_currentCube)
        {
            if (!CubeManager.Instance.ValidateCubeMove(_currentGridIndex, dir))
                return;

            if (OnCubeFollowEvent != null)
                OnCubeFollowEvent(_currentCube, dir);
        }
    }

    private void OnPlayerEndMoveEvent()
    {
        _isMoving = false;

        if (PlayerController.Instance.OnGround)
        {
            _currentGridIndex = RaycastUtils.GetOnlyGrid(_trans.position).GetComponent<GridMember>().GridIndex;
        }
        else
            UpdateCubeReferences();
    }

    private void OnPlayerEndJumpCubeEvent()
    {
        _isMoving = false;

        UpdateCubeReferences();
    }

    private void OnCubeExplosionEvent(GameObject cube)
    {
        if (cube != _currentCube || _isMoving)
            return;

        if (PlayerOnNoCubeEvent != null)
            PlayerOnNoCubeEvent();
    }

    private void UpdateCubeReferences()
    {
        _currentCube = RaycastUtils.PlayerGetCube(_trans.position);

        if (_currentCube)
        {
            CurrentCubeInformation = _currentCube.GetComponent<CubeInformation>();

            if (PlayerOnCubeEvent != null)
                PlayerOnCubeEvent(_currentCube, CurrentCubeInformation.IsInCountDownState);

            _currentGridIndex = CurrentCubeInformation.CurrentIndex;
        }
        else
        {
            if (!PlayerController.Instance.OnGround)
                if (PlayerOnNoCubeEvent != null)
                    PlayerOnNoCubeEvent();
        }
    }
}
