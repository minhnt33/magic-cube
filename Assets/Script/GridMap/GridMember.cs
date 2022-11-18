using UnityEngine;
using System.Collections.Generic;

public class GridMember : MonoBehaviour
{
    [SerializeField]
    private Index _index = new Index();
    public Index GridIndex { get { return _index; } }
    [SerializeField]
    private GameObject _curCube;
    public GameObject CurrentCube { 
        set {
            _curCube = value;

            if (value)
                CurrentCubeInformation = value.GetComponent<CubeInformation>();
            else
                CurrentCubeInformation = null;
        } 
        get{return _curCube;}
    }

    public CubeInformation CurrentCubeInformation { set; get; }

    public Vector3 Waypoint
    {
        get
        {
            return transform.position + Vector3.up;
        }
    }
}
