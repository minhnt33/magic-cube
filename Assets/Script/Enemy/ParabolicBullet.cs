using UnityEngine;
using System.Collections;

public class ParabolicBullet : BaseBulletMovement
{
    public float _timeTravel = 1f;
    public float _angleScalar = 1f;
    public Vector3 _torque;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Move()
    {
        _body.AddTorque(_torque);
        StartCoroutine(ParabolMovementCoroutine(_timeTravel));
    }

    public override void Move(Vector3 direction)
    {
    }

    private IEnumerator ParabolMovementCoroutine(float duration)
    {
        //_body.isKinematic = true;
        Vector3 targetPosition = GridMapManager.Instance.RandomAvailableGrid.transform.position;
        Vector3 startingPosition = _trans.position;

        float timer = duration;
        float progress = 0f;
        float pi = Mathf.PI;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(duration, 0f, timer);

            Vector3 newPosition = Vector3.Lerp(startingPosition, targetPosition, progress);
            newPosition.y = _angleScalar * Mathf.Cos(Mathf.Lerp(-pi / 2, pi / 2, progress));
            _trans.position = newPosition;

            if (progress == 1f)
            {
                //_body.isKinematic = false;
                yield break;
            }

            yield return null;
        }
    }
}
