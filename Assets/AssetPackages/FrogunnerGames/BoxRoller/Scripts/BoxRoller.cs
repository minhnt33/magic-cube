using UnityEngine;
using System.Collections;

public class BoxRoller : MonoBehaviour
{
    [SerializeField]
    private Vector3 _boxSizeRatio = Vector3.one;

    [SerializeField]
    private float _rotatingAngle = 90f;

    [SerializeField]
    private float _movingDuration = 1f;

    [Tooltip("Only use to snap position of 1:1:1 size ratio box on a grid system")]
    [SerializeField]
    private bool _snapPosition;

    [SerializeField]
    private bool _debugPivot;

    private Transform _trans;
    private Transform _pivotPoint;

    private Vector3 _virtureAxisVec = Vector3.zero;  // Virtual axis used to swap axis x, y, z when rotate nonuniform size box for exactly positioning
    private bool _isMoving = false;

    public enum DIRECTION { FORWARFD, BACKWARD, RIGHT, LEFT };
    private DIRECTION _curDir;

    public delegate void StartingRoll(GameObject go);
    public static event StartingRoll StartingRollEvent;

    public delegate void EndRoll(GameObject go);
    public static event EndRoll EndRollEvent;

    void Awake()
    {
        _trans = transform;
        _virtureAxisVec = _boxSizeRatio;

        GameObject pivotGO = new GameObject("Pivot");
        _pivotPoint = pivotGO.transform;
        _pivotPoint.position = _trans.position;
        _pivotPoint.parent = _trans;

        if (_debugPivot)
            pivotGO.AddComponent<PivotGismos>();
    }

    public void RollingForward()
    {
        if (_isMoving)
            return;

        _curDir = DIRECTION.FORWARFD;
        _isMoving = true;

        _pivotPoint.Translate(0, -_virtureAxisVec.y / 2, _virtureAxisVec.z / 2);
        CalculatePivotPoint();

        StartCoroutine(RollingCoroutine(_pivotPoint.position, Vector3.right, _rotatingAngle, _movingDuration));
    }

    public void RollingBackward()
    {
        if (_isMoving)
            return;

        _curDir = DIRECTION.BACKWARD;
        _isMoving = true;

        _pivotPoint.Translate(0, -_virtureAxisVec.y / 2, -_virtureAxisVec.z / 2);
        CalculatePivotPoint();

        StartCoroutine(RollingCoroutine(_pivotPoint.position, -Vector3.right, _rotatingAngle, _movingDuration));
    }

    public void RollingLeft()
    {
        if (_isMoving)
            return;

        _curDir = DIRECTION.LEFT;
        _isMoving = true;

        _pivotPoint.Translate(-_virtureAxisVec.x / 2, -_virtureAxisVec.y / 2, 0);
        CalculatePivotPoint();

        StartCoroutine(RollingCoroutine(_pivotPoint.position, Vector3.forward, _rotatingAngle, _movingDuration));
    }

    public void RollingRight()
    {
        if (_isMoving)
            return;

        _curDir = DIRECTION.RIGHT;
        _isMoving = true;

        _pivotPoint.Translate(_virtureAxisVec.x / 2, -_virtureAxisVec.y / 2, 0);
        CalculatePivotPoint();

        StartCoroutine(RollingCoroutine(_pivotPoint.position, -Vector3.forward, _rotatingAngle, _movingDuration));
    }

    private void CalculatePivotPoint()
    {
        if (_curDir == DIRECTION.LEFT || _curDir == DIRECTION.RIGHT)
        {
            float tmpX = _virtureAxisVec.x;
            _virtureAxisVec.x = _virtureAxisVec.y;
            _virtureAxisVec.y = tmpX;
        }
        else if (_curDir == DIRECTION.FORWARFD || _curDir == DIRECTION.BACKWARD)
        {
            float tmpZ = _virtureAxisVec.z;
            _virtureAxisVec.z = _virtureAxisVec.y;
            _virtureAxisVec.y = tmpZ;
        }
    }

    private IEnumerator RollingCoroutine(Vector3 aPoint, Vector3 aAxis, float aAngle, float aDuration)
    {
        if (StartingRollEvent != null)
            StartingRollEvent(gameObject);

        float timer = aDuration;
        float progress = 0f;
        float prevAngle = 0f;
        float curAngle = 0f;
        while (true)
        {
            // Important to keep pivot rotation fixed 
            _pivotPoint.rotation = Quaternion.identity;

            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(aDuration, 0f, timer);

            curAngle = aAngle * progress;
            _trans.RotateAround(aPoint, aAxis, (curAngle - prevAngle));

            prevAngle = curAngle;

            if (progress == 1f)
            {
                // Make sure the y position is correct
                Vector3 pos = _trans.position;

                if (_snapPosition)
                {
                    pos.x = Mathf.Round(pos.x);
                    pos.z = Mathf.Round(pos.z);
                }

                pos.y = _virtureAxisVec.y / 2;
                _trans.position = pos;

                // Make sure the angles are snaping to rotating degrees.       
                Vector3 vec = _trans.eulerAngles;
                vec.x = Mathf.Round(vec.x / _rotatingAngle) * _rotatingAngle;
                vec.y = Mathf.Round(vec.y / _rotatingAngle) * _rotatingAngle;
                vec.z = Mathf.Round(vec.z / _rotatingAngle) * _rotatingAngle;
                _trans.eulerAngles = vec;

                // move the targetpoint to the center of the box   
                _pivotPoint.position = _trans.position;
                _pivotPoint.rotation = Quaternion.identity;

                // The box is stoped  
                _isMoving = false;

                if (EndRollEvent != null)
                    EndRollEvent(gameObject);

                yield break;

            }
            yield return null;
        }
    }
}