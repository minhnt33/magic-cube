using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CubeGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _magicCubePrefab;
    [SerializeField]
    private GameObject _noMagicCubePrefab;
    [SerializeField]
    private float _maxFrequency;
    [SerializeField]
    private float _cubeMovingUpTime;

    private int _startingCubeNumber;
    [SerializeField]
    private float _currentFrequency = 0f;
    private float[] _frequencyState = { 0f, 25f, 50f, 75f, 100f };
    private CubeManager _cubeManager;
    private GridMapManager _gridManager;
    private Index _mapSize;
    private Coroutine _spawnCor;

    private float CurrentFrequency
    {
        set
        {
            if (_currentFrequency != value)
            {
                _currentFrequency = value;

                if (_spawnCor != null)
                    StopCoroutine(_spawnCor);

                _spawnCor = StartCoroutine(SpawCubeCorountine(_currentFrequency));
            }
        }
    }

    public delegate void OnCubeSpawnCompletely(GameObject cube);
    public static event OnCubeSpawnCompletely OnCubeSpawnCompletelyEvent;

    public delegate void OnCubeSpawnProgress(GameObject cube, float progress);
    public static event OnCubeSpawnProgress OnCubeSpawnProgressEvent;

    public delegate void OnFullGridMap();
    public static event OnFullGridMap OnFullGridMapEvent;

    void OnEnable()
    {
        LevelLoader.OnLevelLoadedEvent += OnLevelLoadedEvent;
        FirstCutSceneCam.OnEndCutSceneEvent += OnEndCutSceneEvent;
    }

    void OnDisable()
    {
        LevelLoader.OnLevelLoadedEvent -= OnLevelLoadedEvent;
        FirstCutSceneCam.OnEndCutSceneEvent -= OnEndCutSceneEvent;
    }

    void Awake()
    {
        _cubeManager = CubeManager.Instance;
        _gridManager = GridMapManager.Instance;

        _mapSize = _gridManager.MapSize;
        GenerateCubeMap();
    }

    private GameType _gameType;
    private Difficulties _difficulty;

    private void OnLevelLoadedEvent(LevelInformation info)
    {
        _gameType = info.GameType;
        _difficulty = info.Difficulty;
    }

    private void OnEndCutSceneEvent()
    {
        if (_gameType == GameType.NORMAL || _gameType == GameType.TIME_ATTACK)
            return;

        CurrentFrequency = CalculateFrequency();
    }

    private float CalculateFrequency()
    {
        float totalCube = _gridManager.UnavailableGridList.Count;
        float totalGrid = _gridManager.RealGridList.Count;
        float fillPercent = 100f * totalCube / totalGrid;
        int indexState = Mathf.FloorToInt(Mathf.FloorToInt(fillPercent) / 25) + 1;
        return _frequencyState[indexState == _frequencyState.Length ? indexState - 1 : indexState] * _maxFrequency / 100f;
    }

    private void GenerateCubeMap()
    {
        LevelInformation info = LevelLoader.Instance.LoadedLevelInfo;
        _startingCubeNumber = info.StartingCubeNumber;

        // Always generate a cube under player
        CreateANoMagicCube(info.PlayerInitPosition.Row, info.PlayerInitPosition.Column, _noMagicCubePrefab);

        for (int cubeCount = 0; cubeCount < _startingCubeNumber - 1; cubeCount++)
        {
            CreateARandomMovingUpCube();
        }

        foreach (GridMember grid in _gridManager.UnavailableGridList)
        {
            GameObject currentCube = _gridManager.GetGridAt(grid.GridIndex).CurrentCube;

            if (!currentCube)
                continue;

            //Delete duplicate cubes
            _cubeManager.DeleteAdjacentCubes(grid.GridIndex.Row, grid.GridIndex.Column);
        }

    }

    private GameObject CreateANoMagicCube(int row, int col, GameObject prefab)
    {
        GridMember grid = _gridManager.GetGridAt(row, col);
        if (!grid)
            return null;

        if (grid.CurrentCube)
            return null;

        GameObject cube = PoolingHelper.Instance.InstantiatePrefab(prefab, grid.Waypoint, Quaternion.identity);

        grid.CurrentCube = cube;
        _gridManager.UnavailableGrid(grid);

        // Group cube for easyly managing
        cube.transform.parent = _cubeManager.transform;

        return cube;
    }

    private GameObject CreateACube(int row, int col, GameObject prefab, CubeFace face)
    {
        GridMember grid = _gridManager.GetGridAt(row, col);
        if (!grid)
            return null;

        if (grid.CurrentCube)
            return null;

        GameObject cube = PoolingHelper.Instance.InstantiatePrefab(prefab, grid.Waypoint, Quaternion.identity);

        _cubeManager.SetFace(cube.transform, face);

        grid.CurrentCube = cube;
        _gridManager.UnavailableGrid(grid);

        // Group cube for easyly managing
        cube.transform.parent = _cubeManager.transform;

        return cube;
    }

    private GameObject CreateACube(Index index, GameObject prefab, CubeFace face)
    {
        return CreateACube(index.Row, index.Column, prefab, face);
    }

    private GameObject CreateARandomCube()
    {
        return CreateACube(Random.Range(0, _mapSize.Row), Random.Range(0, _mapSize.Column), _magicCubePrefab, EnumerationUtils.RandomCubeFace);
    }

    private GameObject CreateARandomAvailableCube()
    {
        return CreateACube(_gridManager.RandomAvailableGrid.GridIndex, _magicCubePrefab, EnumerationUtils.RandomCubeFace);
    }

    private void CreateARandomMovingUpCube()
    {
        Index index = _gridManager.RandomAvailableGrid.GridIndex;
        CreateAMovingUpCube(index.Row, index.Column, _magicCubePrefab, EnumerationUtils.RandomCubeFace);
    }

    private GameObject CreateAMovingUpCube(int row, int col, GameObject prefab, CubeFace face)
    {
        GameObject cube = CreateACube(row, col, prefab, face);

        Transform cubeTrans = cube.transform;

        Vector3 pos = cubeTrans.position;
        pos.y = -0.3f;
        cubeTrans.position = pos;

        StartCoroutine(CubeMovingUpCoroutine(cubeTrans, 0.8f, _cubeMovingUpTime));

        return cube;
    }

    private IEnumerator SpawCubeCorountine(float frequency)
    {
        WaitForSeconds wait = new WaitForSeconds(frequency);
        while (true)
        {
            CurrentFrequency = CalculateFrequency();

            if (_gridManager.FullGrid && _cubeManager.HasCountDownedCubes)
            {
                yield return null;
            }

            if (!_cubeManager.HasCountDownedCubes && _gridManager.FullGrid)
            {
                if (OnFullGridMapEvent != null)
                    OnFullGridMapEvent();
                yield break;
            }

            CreateARandomMovingUpCube();
            yield return wait;
        }
    }

    private IEnumerator CubeMovingUpCoroutine(Transform cubeTrans, float dis, float secs)
    {
        float timer = secs;
        float progress = 0f;
        Vector3 startingPos = cubeTrans.position;
        Vector3 targetPos = startingPos;
        targetPos.y = targetPos.y + dis;

        while (true)
        {
            if (OnCubeSpawnProgressEvent != null)
                OnCubeSpawnProgressEvent(cubeTrans.gameObject, progress);

            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(secs, 0f, timer);

            cubeTrans.position = Vector3.Lerp(startingPos, targetPos, progress);

            if (progress == 1f)
            {
                if (OnCubeSpawnCompletelyEvent != null)
                    OnCubeSpawnCompletelyEvent(cubeTrans.gameObject);

                yield break;
            }

            yield return null;
        }
    }
}
