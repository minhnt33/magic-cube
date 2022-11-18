using UnityEngine;
using System.Collections;

public class BoxMovement : MonoBehaviour
{
    private BoxRoller _boxRoller;

    void OnEnable()
    {
        PlayerCubeControl.OnCubeFollowEvent += OnCubeFollowEvent;
    }

    void OnDisable()
    {
        PlayerCubeControl.OnCubeFollowEvent -= OnCubeFollowEvent;
    }

    void Awake()
    {
        _boxRoller = GetComponent<BoxRoller>();
    }

    private void OnCubeFollowEvent(GameObject curCube, Direction dir)
    {
        if (curCube != gameObject)
            return;

        if (dir == Direction.FORWARD)
            _boxRoller.RollingForward();
        else if (dir == Direction.BACKWARD)
            _boxRoller.RollingBackward();
        else if (dir == Direction.LEFT)
            _boxRoller.RollingLeft();
        else if (dir == Direction.RIGHT)
            _boxRoller.RollingRight();
    }
}
