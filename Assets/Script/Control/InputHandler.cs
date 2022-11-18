using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour
{

    public delegate void OnPlayerMovement(Direction direction);
    public static event OnPlayerMovement OnPlayerMovementEvent;

    private void Move(Direction dir)
    {
        if (OnPlayerMovementEvent != null)
            OnPlayerMovementEvent(dir);
    }

    public void MoveForward()
    {
        Move(Direction.FORWARD);
    }

    public void MoveBackward()
    {
        Move(Direction.BACKWARD);
    }

    public void MoveLeft()
    {
        Move(Direction.LEFT);
    }

    public void MoveRight()
    {
        Move(Direction.RIGHT);
    }
}
