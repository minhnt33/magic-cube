using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelLocation : System.Object
{
    [SerializeField]
    private LevelLocationType _location;
    public LevelLocationType Location { get { return _location; } }

    [SerializeField]
    private Material _gridMat;
    public Material GridMat { get { return _gridMat; } }

    [SerializeField]
    private GameObject _groundMat;
    public GameObject GroundTerrain { get { return _groundMat; } }

    [SerializeField]
    private Material _skybox;
    public Material Skybox { get { return _skybox; } }
}
