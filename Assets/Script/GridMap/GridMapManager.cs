using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LevelGenerator))]
public class GridMapManager : MonoBehaviour
{
    private static GridMapManager _instance = null;

    public static GridMapManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private GridMember[,] _gridArray;

    private Index _mapSize;
    public Index MapSize { get { return _mapSize; } }
    [SerializeField]
    private List<GridMember> _unavailableGrid;
    public List<GridMember> UnavailableGridList { get { return _unavailableGrid; } }

    private List<GridMember> _availableGrid;
    public List<GridMember> AvailableGridList { get { return _availableGrid; } }

    private List<GridMember> _realGridList;
    public List<GridMember> RealGridList { get { return _realGridList; } }

    public GridMember RandomGrid
    {
        get { return _realGridList[Random.Range(0, _realGridList.Count)]; }
    }

    public GridMember RandomAvailableGrid
    {
        get { return _availableGrid[Random.Range(0, _availableGrid.Count)]; }
    }

    public GridMember RandomUnavailableGrid
    {
        get { return _unavailableGrid[Random.Range(0, _unavailableGrid.Count)]; }
    }

    public bool FullGrid { get { return _availableGrid.Count == 0; } }
    public bool EmptyGrid { get { return _unavailableGrid.Count == 1; } }

    private bool[,] _visitedGrid;

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        LevelInformation levelInfo = LevelLoader.Instance.LoadedLevelInfo;
        _mapSize = new Index(levelInfo.MapRow, levelInfo.MapColumn);

        InitGridArray();
    }

    private void InitGridArray()
    {
        int maxGridNumber = _mapSize.Row * _mapSize.Column;

        _realGridList = new List<GridMember>(maxGridNumber);
        _availableGrid = new List<GridMember>(maxGridNumber);
        _unavailableGrid = new List<GridMember>(maxGridNumber);

        _gridArray = new GridMember[_mapSize.Row, _mapSize.Column];
        _visitedGrid = new bool[_mapSize.Row, _mapSize.Column];

        foreach (Transform child in transform)
        {
            GridMember gridMember = child.GetComponent<GridMember>();
            if (gridMember)
            {
                _gridArray[gridMember.GridIndex.Row, gridMember.GridIndex.Column] = gridMember;

                // Add grid to real list
                _realGridList.Add(gridMember);

                // Add grid to available grid
                _availableGrid.Add(gridMember);
            }
        }
    }

    public void FreeGrid(GridMember grid)
    {
        if (grid == null)
            return;

        AvailableGrid(grid);
    }

    public void UpdateCubeReferenceInGrid(GameObject cube, Index currentIndex, Direction dir)
    {
        GridMember nextGrid = GetGridNeighborAt(currentIndex.Row, currentIndex.Column, dir);

        if (!nextGrid)
            return;

        nextGrid.CurrentCube = cube;
        UnavailableGrid(nextGrid);
    }

    public void UpdateCubeReferenceInGrid(Transform cubeTrans, Direction dir)
    {
        GameObject currentGrid = RaycastUtils.GetGrid(cubeTrans.position);

        if (!currentGrid)
            return;

        GridMember gridMember = currentGrid.GetComponent<GridMember>();
        gridMember.CurrentCube = null;

        Index index = gridMember.GridIndex;
        if (dir == Direction.FORWARD)
            _gridArray[index.Row + 1, index.Column].CurrentCube = cubeTrans.gameObject;
        else if (dir == Direction.BACKWARD)
            _gridArray[index.Row - 1, index.Column].CurrentCube = cubeTrans.gameObject;
        else if (dir == Direction.LEFT)
            _gridArray[index.Row, index.Column - 1].CurrentCube = cubeTrans.gameObject;
        else if (dir == Direction.RIGHT)
            _gridArray[index.Row, index.Column + 1].CurrentCube = cubeTrans.gameObject;
    }

    // Queue based Flood Fill
    private Queue<GridMember> gridQueue = new Queue<GridMember>();
    public void floodFill(int row, int col, List<GridMember> adjacentList)
    {
        if (!CheckIndex(row, col))
            return;

        adjacentList.Clear();
        gridQueue.Clear();

        for (int i = 0; i < _mapSize.Row; i++)
            for (int k = 0; k < _mapSize.Column; k++)
                _visitedGrid[i, k] = false;

        GridMember startingGrid = GetGridAt(row, col);
        GameObject startingCube = startingGrid.CurrentCube;

        if (!startingCube)
            return;

        CubeInformation baseInfo = startingCube.GetComponent<CubeInformation>();
        if (baseInfo.GetCubeType != CubeType.MAGIC)
            return;

        CubeFace startingFace = baseInfo.CurrentFace;
        gridQueue.Enqueue(startingGrid);

        GridMember tempGrid;
        while (gridQueue.Count > 0)
        {
            tempGrid = gridQueue.Dequeue();

            if (!tempGrid)
                continue;

            if (!tempGrid.CurrentCube)
                continue;

            Index index = tempGrid.GridIndex;

            if (_visitedGrid[index.Row, index.Column])
                continue;

            adjacentList.Add(tempGrid);

            GridMember upGrid = GetGridAt(index.Row + 1, index.Column);
            GridMember downGrid = GetGridAt(index.Row - 1, index.Column);
            GridMember leftGrid = GetGridAt(index.Row, index.Column - 1);
            GridMember rightGrid = GetGridAt(index.Row, index.Column + 1);

            _visitedGrid[index.Row, index.Column] = true;

            EnqueueGrid(upGrid, startingFace);
            EnqueueGrid(downGrid, startingFace);
            EnqueueGrid(rightGrid, startingFace);
            EnqueueGrid(leftGrid, startingFace);
        }
    }

    private void EnqueueGrid(GridMember grid, CubeFace checkedFace)
    {
        if (grid && grid.CurrentCube)
        {
            CubeInformation baseInfo = grid.CurrentCube.GetComponent<CubeInformation>();

            if (baseInfo.GetCubeType != CubeType.MAGIC)
                return;

            if (baseInfo.CurrentFace != checkedFace)
                return;

            gridQueue.Enqueue(grid);
        }
    }

    // Recursive Flood Fill
    public void FindAdjacentSameFaceCubes(int row, int col, List<GridMember> adjacentList)
    {
        if (row < 0 || col < 0 || row >= _mapSize.Row || col >= _mapSize.Column)
            return;

        adjacentList.Clear();

        GridMember currentGrid = GetGridAt(row, col);

        if (!currentGrid || !currentGrid.CurrentCube)
            return;

        CubeInformation face = currentGrid.CurrentCube.GetComponent<CubeInformation>();
        CubeFace currentFace = face.CurrentFace;

        adjacentList.Add(currentGrid);

        GameObject downCube = null;
        GameObject upCube = null;
        GameObject leftCube = null;
        GameObject rightCube = null;

        if (row - 1 >= 0 && _gridArray[row - 1, col])
            downCube = _gridArray[row - 1, col].CurrentCube;
        if (row + 1 < _mapSize.Row && _gridArray[row + 1, col])
            upCube = _gridArray[row + 1, col].CurrentCube;
        if (col - 1 >= 0 && _gridArray[row, col - 1])
            leftCube = _gridArray[row, col - 1].CurrentCube;
        if (col + 1 < _mapSize.Column && _gridArray[row, col + 1])
            rightCube = _gridArray[row, col + 1].CurrentCube;

        if ((downCube && currentFace == downCube.GetComponent<CubeInformation>().CurrentFace) || (upCube && currentFace == upCube.GetComponent<CubeInformation>().CurrentFace) || (rightCube && currentFace == rightCube.GetComponent<CubeInformation>().CurrentFace) || (leftCube && currentFace == leftCube.GetComponent<CubeInformation>().CurrentFace))
        {
            FindNeighborSameFaceCubes(row - 1, col, currentFace, Direction.BACKWARD, adjacentList);

            //  Recursive call for down
            FindNeighborSameFaceCubes(row + 1, col, currentFace, Direction.FORWARD, adjacentList);

            //  Recursive call for left
            FindNeighborSameFaceCubes(row, col - 1, currentFace, Direction.RIGHT, adjacentList);

            //  Recursive call for right
            FindNeighborSameFaceCubes(row, col + 1, currentFace, Direction.LEFT, adjacentList);
        }
    }

    private void FindNeighborSameFaceCubes(int row, int col, CubeFace face, Direction direction, List<GridMember> adjacentList)
    {
        //  Check if it is on the board
        if (row < 0 || row >= _mapSize.Row || col < 0 || col >= _mapSize.Column)
            return;

        GridMember grid = GetGridAt(row, col);

        GameObject currentCube = null;

        if (grid)
            currentCube = grid.CurrentCube;

        if (!currentCube)
            return;

        CubeFace currentFace = currentCube.GetComponent<CubeInformation>().CurrentFace;

        //  Check if it has the same color
        if (currentFace != face)
            return;

        adjacentList.Add(grid);

        //  If we weren't told to not go back up, check up
        if (direction != Direction.FORWARD)
            FindNeighborSameFaceCubes(row - 1, col, face, Direction.BACKWARD, adjacentList);
        //  If we weren't told to not go back down, check down
        if (direction != Direction.BACKWARD)
            FindNeighborSameFaceCubes(row + 1, col, face, Direction.FORWARD, adjacentList);
        //  If we weren't told to not go back left, check left
        if (direction != Direction.LEFT)
            FindNeighborSameFaceCubes(row, col - 1, face, Direction.RIGHT, adjacentList);
        //  If we weren't told to not go back right, check right
        if (direction != Direction.RIGHT)
            FindNeighborSameFaceCubes(row, col + 1, face, Direction.LEFT, adjacentList);
    }

    public bool HasCubeNeighborAt(int row, int col, Direction direction)
    {
        return GetGridNeighborAt(row, col, direction).CurrentCube;
    }

    public GridMember GetGridNeighborAt(int row, int col, Direction direction)
    {
        if (!CheckIndex(row, col))
            return null;

        if (direction == Direction.FORWARD)
            return GetGridAt(row + 1, col);
        else if (direction == Direction.BACKWARD)
            return GetGridAt(row - 1, col);
        else if (direction == Direction.LEFT)
            return GetGridAt(row, col - 1);
        else if (direction == Direction.RIGHT)
            return GetGridAt(row, col + 1);

        return null;
    }

    public void UnavailableGrid(GridMember grid)
    {
        _unavailableGrid.Add(grid);
        _availableGrid.Remove(grid);
    }

    public void UnavailableGrid(GameObject cube)
    {
        GridMember grid = GetGridAt(cube.GetComponent<CubeInformation>().CurrentIndex);

        _unavailableGrid.Add(grid);
        _availableGrid.Remove(grid);
    }

    private void AvailableGrid(GridMember grid)
    {
        grid.CurrentCube = null;
        _availableGrid.Add(grid);
        _unavailableGrid.Remove(grid);
    }

    public void AvailableGrid(GameObject cube)
    {
        GridMember grid = GetGridAt(cube.GetComponent<CubeInformation>().CurrentIndex);

        grid.CurrentCube = null;
        _availableGrid.Add(grid);
        _unavailableGrid.Remove(grid);
    }

    public GridMember GetGridAt(Index index)
    {
        if (!CheckIndex(index.Row, index.Column))
            return null;

        return _gridArray[index.Row, index.Column];
    }

    public GridMember GetGridAt(int row, int column)
    {
        if (!CheckIndex(row, column))
            return null;

        return _gridArray[row, column];
    }

    private bool CheckIndex(int row, int col)
    {
        if (row >= 0 && row < _mapSize.Row && col >= 0 && col < _mapSize.Column)
            return true;

        return false;
    }
}
