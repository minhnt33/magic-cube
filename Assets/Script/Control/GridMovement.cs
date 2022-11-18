using UnityEngine;
using System.Collections;

public abstract class GridMovement : MonoBehaviour
{
    [SerializeField]
    private float _unitDistance;
    [SerializeField]
    private float movingDuration;
    [SerializeField]
    private float _rotatingDuration;
    [SerializeField]
    protected bool _canJumpOnCube;

    protected Transform _trans;
    private Direction _curDirection;

    private bool _isMoving = false;
    private bool _isRotating = false;

    public delegate void OnGridMoveSuccess(GameObject go, Direction dir);
    public static event OnGridMoveSuccess OnGridMoveSuccessEvent;

    public delegate void OnGridEndMove(GameObject go);
    public static event OnGridEndMove OnGridEndMoveEvent;

    public delegate void OnEndJumpCube(GameObject go);
    public static event OnEndJumpCube OnEndJumpCubeEvent;

    public delegate void HasCubeForward(GameObject go, Transform cubeTrans);
    public static event HasCubeForward HasCubeForwardEvent;

    [SerializeField]
    protected bool _isOnGround = false;
    public bool IsOnGround { set { _isOnGround = value; } get { return _isOnGround; } }

    [SerializeField]
    protected Index _currentGridIndex;
    public Index CurrentIndex { set { _currentGridIndex = value; } get { return _currentGridIndex; } }

    public abstract bool CheckingMovement();

    protected void MoveForward()
    {
        _curDirection = Direction.FORWARD;
        Rotate(0f, _rotatingDuration);
        Move(Vector3.forward, _unitDistance, movingDuration);
    }

    protected void MoveBackward()
    {
        _curDirection = Direction.BACKWARD;
        Rotate(180f, _rotatingDuration);
        Move(Vector3.back, _unitDistance, movingDuration);
    }

    protected void MoveLeft()
    {
        _curDirection = Direction.LEFT;
        Rotate(-90f, _rotatingDuration);
        Move(Vector3.left, _unitDistance, movingDuration);
    }

    protected void MoveRight()
    {
        _curDirection = Direction.RIGHT;
        Rotate(90f, _rotatingDuration);
        Move(Vector3.right, _unitDistance, movingDuration);
    }

    private void Rotate(float angle, float secs)
    {
        if (_isRotating)
            return;

        StartCoroutine(RotatingCoroutine(angle, secs));
    }

    private void Move(Vector3 dir, float dis, float secs)
    {
        if (_isMoving)
            return;

        if (_canJumpOnCube)
            if (_isOnGround)
            {
                GameObject forwardCube = RaycastUtils.PlayerGetCubeForward(_trans.position, dir);
                if (forwardCube)
                {
                    if (HasCubeForwardEvent != null)
                        HasCubeForwardEvent(gameObject, forwardCube.transform);

                    return;
                }
            }
            else
            {
                if (!CubeManager.Instance.ValidateActorMove(_currentGridIndex, _curDirection))
                    return;
            }
        else
            if (!CubeManager.Instance.ValidateActorMove(_currentGridIndex, _curDirection))
                return;

        //if (!CheckingMovement())
        //    return;

        StartCoroutine(MovingCoroutine(dir, dis, secs));

        if (OnGridMoveSuccessEvent != null)
            OnGridMoveSuccessEvent(gameObject, _curDirection);
    }

    private void UpdateActorGridIndex()
    {
        if (_curDirection == Direction.FORWARD)
            _currentGridIndex.SetIndex(_currentGridIndex.Row + 1, _currentGridIndex.Column);
        else if (_curDirection == Direction.BACKWARD)
            _currentGridIndex.SetIndex(_currentGridIndex.Row - 1, _currentGridIndex.Column);
        else if (_curDirection == Direction.LEFT)
            _currentGridIndex.SetIndex(_currentGridIndex.Row, _currentGridIndex.Column - 1);
        else if (_curDirection == Direction.RIGHT)
            _currentGridIndex.SetIndex(_currentGridIndex.Row, _currentGridIndex.Column + 1);
    }


    public void MoveOnCube(Transform cubeTrans)
    {
        if (_isMoving)
            return;

        StartCoroutine(MoveOnCubeCorountine(_trans.position, cubeTrans, 0.6f));
    }

    private IEnumerator MoveOnCubeCorountine(Vector3 from, Transform toTrans, float secs)
    {
        _isMoving = true;

        float timer = secs;
        float progress = 0f;
        Vector3 to = Vector3.zero;

        while (true)
        {
            timer -= Time.deltaTime;

            // If target is null. Use the last its position for bullet
            to = toTrans ? toTrans.position + Vector3.up : to;

            // smooth traveling time
            progress = Mathf.InverseLerp(secs, 0f, timer);

            //set projectile position
            Vector3 newPosition = Vector3.Lerp(from, to, progress);

            _trans.position = newPosition;

            if (progress == 1f || toTrans == null)
            {
                _isMoving = false;

                if (OnEndJumpCubeEvent != null)
                    OnEndJumpCubeEvent(gameObject);

                UpdateActorGridIndex();
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator MovingCoroutine(Vector3 dir, float dis, float secs)
    {
        _isMoving = true;
        float timer = secs;
        float progress = 0f;
        Vector3 startingPos = _trans.position;
        Vector3 destination = startingPos + dir * dis;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(secs, 0f, timer);

            _trans.position = Vector3.Lerp(startingPos, destination, progress);

            if (progress == 1)
            {
                if (OnGridEndMoveEvent != null)
                    OnGridEndMoveEvent(gameObject);

                UpdateActorGridIndex();
                _isMoving = false;
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator RotatingCoroutine(float facingAngle, float secs)
    {
        _isRotating = true;
        float timer = secs;
        float progress = 0f;
        Quaternion startingQuat = _trans.rotation;
        Quaternion targetQuat = Quaternion.Euler(0f, facingAngle, 0f);

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(secs, 0f, timer);

            _trans.rotation = Quaternion.Slerp(startingQuat, targetQuat, progress);

            if (progress == 1)
            {
                _isRotating = false;
                yield break;
            }

            yield return null;
        }
    }
}
