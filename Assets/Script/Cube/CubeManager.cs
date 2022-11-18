using UnityEngine;
using System.Collections.Generic;

public class CubeManager : MonoBehaviour
{
    private static CubeManager _instance = null;

    public static CubeManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private int _minAdjacent = 3;
    [SerializeField]
    private CubeAttackInfo[] _cubeAttackInfo = new CubeAttackInfo[6];

    private GridMapManager _gridManager;
    private List<GridMember> _adjacentList;
    [SerializeField]
    private List<GameObject> _countDownedCubeList;

    public bool HasCountDownedCubes { get { return _countDownedCubeList.Count > 0; } }
    public int TotalCountdownCube { get { return _countDownedCubeList.Count; } }

    public delegate void OnCubeBlockExplode(GameObject explodedCube, GridMember grid, int cubeNumber);
    public static event OnCubeBlockExplode OnCubeBlockExplodeEvent;

    void OnEnable()
    {
        CubeExplosion.OnCubeExplosionEvent += OnCubeExplosionEvent;
    }

    void OnDisable()
    {
        CubeExplosion.OnCubeExplosionEvent -= OnCubeExplosionEvent;
    }

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _gridManager = GridMapManager.Instance;
        _adjacentList = new List<GridMember>();
        _countDownedCubeList = new List<GameObject>();
    }

    private void OnCubeExplosionEvent(GameObject cube)
    {
        _countDownedCubeList.Remove(cube);
    }

    public void AddCubeToCountdownList(GameObject cube)
    {
        _countDownedCubeList.Add(cube);
    }

    public void DeleteAdjacentCubes(int row, int col)
    {
        _gridManager.floodFill(row, col, _adjacentList);

        if (_adjacentList.Count >= _minAdjacent)
        {
            foreach (GridMember grid in _adjacentList)
            {
                CubeInformation info = grid.CurrentCube.GetComponent<CubeInformation>();

                if (info.IsInCountDownState)
                    continue;

                info.IsInCountDownState = true;

                if (OnCubeBlockExplodeEvent != null)
                    OnCubeBlockExplodeEvent(grid.CurrentCube, grid, _adjacentList.Count);
            }
        }
    }

    public void NoMatchingAtStart(int row, int col)
    {
        _gridManager.FindAdjacentSameFaceCubes(row, col, _adjacentList);

        int totalCube = _adjacentList.Count;

        if (totalCube >= _minAdjacent)
            for (int i = 0; i < _adjacentList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    CubeFace currentFace = _adjacentList[i].GetComponent<CubeInformation>().CurrentFace;
                    SetFace(_adjacentList[i].transform, EnumerationUtils.RandomCubeFaceException(currentFace));
                }
            }
        else
            _adjacentList.Clear();
    }

    public CubeAttackInfo GetAttackInfoBySkill(CubeSkill skill)
    {
        for (int i = 0; i < _cubeAttackInfo.Length; i++)
        {
            if (skill == _cubeAttackInfo[i].SkillType)
                return _cubeAttackInfo[i];
        }

        return null;
    }

    public CubeAttackInfo GetAttackInfo(GameObject cube)
    {
        CubeSkill skill = cube.GetComponent<CubeInformation>().CurrentSkill;

        for (int i = 0; i < _cubeAttackInfo.Length; i++)
        {
            if (skill == _cubeAttackInfo[i].SkillType)
                return _cubeAttackInfo[i];
        }

        return null;
    }

    public void SetFace(Transform cubeTrans, CubeFace topFace)
    {
        cubeTrans.localRotation = Quaternion.identity;
        Transform pivot = cubeTrans.GetChild(0);
        pivot.localRotation = Quaternion.identity;

        if (topFace == CubeFace.BACKWARD_FACE)
            RotateCube(cubeTrans, pivot, Vector3.right, 90f);
        else if (topFace == CubeFace.BOTTOM_FACE)
            RotateCube(cubeTrans, pivot, Vector3.right, 180f);
        else if (topFace == CubeFace.FORWARD_FACE)
            RotateCube(cubeTrans, pivot, Vector3.right, -90f);
        else if (topFace == CubeFace.RIGHT_FACE)
            RotateCube(cubeTrans, pivot, Vector3.forward, 90f);
        else if (topFace == CubeFace.LEFT_FACE)
            RotateCube(cubeTrans, pivot, Vector3.forward, -90f);

        CubeInformation cubeFace = cubeTrans.GetComponent<CubeInformation>();

        if (!cubeFace)
            return;

        cubeFace.CurrentFace = topFace;
    }

    private void RotateCube(Transform cubeTrans, Transform pivotTrans, Vector3 axis, float angle)
    {
        cubeTrans.RotateAround(cubeTrans.position, axis, angle);

        // Important to keep rotation of pivot point is identity
        pivotTrans.RotateAround(cubeTrans.position, axis, -angle);
    }

    public bool ValidateActorMove(Index currentIndex, Direction dir)
    {
        GridMember grid = _gridManager.GetGridNeighborAt(currentIndex.Row, currentIndex.Column, dir);

        if (grid)
            if (grid.CurrentCube)
                if (grid.CurrentCubeInformation.IsInSpawningState)
                    return false;
                else
                    return true;
            else
                return true;

        return false;
    }


    public bool ValidateCubeMove(Index currentIndex, Direction dir)
    {
        GridMember grid = _gridManager.GetGridNeighborAt(currentIndex.Row, currentIndex.Column, dir);

        if (grid)
            if (grid.CurrentCube == null)
                return true;

        return false;
    }
}
