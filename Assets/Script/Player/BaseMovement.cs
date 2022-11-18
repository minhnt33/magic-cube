using UnityEngine;
using System.Collections;

public class BaseMovement : MonoBehaviour
{

    [SerializeField]
    protected float _unitDistance;
    [SerializeField]
    protected float movingDuration;
    [SerializeField]
    protected float _rotatingDuration;

    private Coroutine _slowCor;

    public void SlowSpeed(float percentage, float duration)
    {
        if (_slowCor != null)
            return;

        _slowCor = StartCoroutine(SlowCoroutine(percentage, duration));
    }

    private IEnumerator SlowCoroutine(float percentage, float duration)
    {
        float originalSpeed = movingDuration;
        movingDuration = movingDuration * (1 + percentage);

        yield return new WaitForSeconds(duration);

        movingDuration = originalSpeed;
        _slowCor = null;
        yield break;
    }
}
