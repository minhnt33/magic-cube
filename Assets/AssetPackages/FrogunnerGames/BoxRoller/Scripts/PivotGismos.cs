using UnityEngine;
using System.Collections;

public class PivotGismos : MonoBehaviour
{
    [SerializeField]
    private Color gizmocolor = Color.red;
    [SerializeField]
    private float radius = 0.1f;

    private Transform _trans;

    void Awake()
    {
        _trans = transform;
    }

    void OnDrawGizmos()
    {
        if (_trans)
        {
            Gizmos.color = gizmocolor;
            Gizmos.DrawSphere(_trans.position, radius);
        }
    }
}