using UnityEngine;
using System.Collections;

public class PlayerMovement : BaseMovement
{
    private Transform _trans;
    private Direction _curDirection;
    private PlayerCubeControl _playerCubeControl;

    private bool _isMoving = false;
    private bool _isRotating = false;
    private bool _isStun = false;

    public delegate void OnPlayerMoveSuccess(Direction dir);
    public static event OnPlayerMoveSuccess OnPlayerMoveSuccessEvent;

    public delegate void OnPlayerEndMove();
    public static event OnPlayerEndMove OnPlayerEndMoveEvent;

    public delegate void OnPlayerEndJumpCube();
    public static event OnPlayerEndJumpCube OnPlayerEndJumpCubeEvent;

    public delegate void HasCubeForward(Transform cubeTrans);
    public static event HasCubeForward HasCubeForwardEvent;

    void OnEnable()
    {
        InputHandler.OnPlayerMovementEvent += OnPlayerMovementEvent;
        EffectStun.OnStunEffectEvent += OnStunEvent;
    }

    void OnDisable()
    {
        InputHandler.OnPlayerMovementEvent -= OnPlayerMovementEvent;
        EffectStun.OnStunEffectEvent -= OnStunEvent;
    }

    // Use this for initialization
    void Awake()
    {
        _trans = transform;
        _playerCubeControl = GetComponent<PlayerCubeControl>();
    }

    private void OnStunEvent(GameObject target, float duration)
    {
        if (target != gameObject)
            return;

        StartCoroutine(DisableMovement(duration));
    }

    private IEnumerator DisableMovement(float duration)
    {
        _isStun = true;
        yield return new WaitForSeconds(duration);
        _isStun = false;
        yield break;
    }

    private void OnPlayerMovementEvent(Direction dir)
    {
        if (_isStun)
            return;

        _curDirection = dir;

        if (dir == Direction.FORWARD)
        {
            Rotate(0f, _rotatingDuration);
            Move(Vector3.forward, _unitDistance, movingDuration);
        }
        else if (dir == Direction.BACKWARD)
        {
            Rotate(180f, _rotatingDuration);
            Move(Vector3.back, _unitDistance, movingDuration);
        }

        else if (dir == Direction.LEFT)
        {
            Rotate(-90f, _rotatingDuration);
            Move(Vector3.left, _unitDistance, movingDuration);
        }
        else if (dir == Direction.RIGHT)
        {
            Rotate(90f, _rotatingDuration);
            Move(Vector3.right, _unitDistance, movingDuration);
        }
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

        if (!CubeManager.Instance.ValidateActorMove(_playerCubeControl.CurrentGridIndex, _curDirection))
            return;

        if (PlayerController.Instance.OnGround)
        {
            GameObject forwardCube = RaycastUtils.PlayerGetCubeForward(_trans.position, dir);
            if (forwardCube)
            {
                if (HasCubeForwardEvent != null)
                    HasCubeForwardEvent(forwardCube.transform);

                return;
            }
        }

        StartCoroutine(MovingCoroutine(dir, dis, secs));

        if (OnPlayerMoveSuccessEvent != null)
            OnPlayerMoveSuccessEvent(_curDirection);
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

                if (OnPlayerEndJumpCubeEvent != null)
                    OnPlayerEndJumpCubeEvent();

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
                if (OnPlayerEndMoveEvent != null)
                    OnPlayerEndMoveEvent();

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
