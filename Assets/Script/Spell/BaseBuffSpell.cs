using UnityEngine;
using System.Collections;

public abstract class BaseBuffSpell : MonoBehaviour
{

    public abstract void Buff();

    protected virtual void OnEnable()
    {
        CubeExplosion.OnCubeExplosionEvent += OnCubeExplosionEvent;
    }

    protected virtual void OnDisable()
    {
        CubeExplosion.OnCubeExplosionEvent -= OnCubeExplosionEvent;
    }

    private void OnCubeExplosionEvent(GameObject cube)
    {
        Buff();
    }
}
