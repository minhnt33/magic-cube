using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using MadLevelManager;
using SWS;

public struct LevelInformation
{
    public GameType GameType { set; get; }

    public int MapRow { set; get; }

    public int MapColumn { set; get; }

    public Index PlayerInitPosition { set; get; }

    public int StartingCubeNumber { set; get; }

    public Difficulties Difficulty { set; get; }

    public LevelLocation LevelLocation { set; get; }

    public int TimeAttackLimit { set; get; }

    public int[] StarLevelConditions { set; get; }

    public PathManager[] EnemyPaths { set; get; }

    public PathManager[] BossPaths { set; get; }

    public LoadedEnemy[] Enemies { set; get; }

    public float CameraSize { set; get; }
}

public struct LoadedEnemy
{
    public GameObject Prefab;
    public int Count;
    public bool IsBoss;
    public int[] PathIndexs;
}

public class LevelLoader : MonoBehaviour
{
    private static LevelLoader _instance = null;

    public static LevelLoader Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private GameObject _gridPrefab;
    private int _gridSize;

    [SerializeField]
    private LevelLocation[] _levelLocationArray;

    [SerializeField]
    private EnemyInformation[] _allEnemyInfo;

    private int[,] _mapBinary;

    private Transform _trans;

    private LevelInformation _levelInfo;
    public LevelInformation LoadedLevelInfo { get { return _levelInfo; } }

    public delegate void OnLevelLoaded(LevelInformation info);
    public static event OnLevelLoaded OnLevelLoadedEvent;

    private string _boundaryLayerName;
    private bool _boundryIsTrigger;
    private float _boundaryToMapBorder;
    private GameObject[] _boundaryArray;
    private Index _mapSize;

    private List<Vector3[]> _enemyPathList;
    private List<Vector3[]> _bossPathList;

    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _trans = transform;

        _levelInfo = new LevelInformation();

        LoadMap(MadLevel.arguments);

        CreateMap();

        if (_enemyPathList != null)
            _levelInfo.EnemyPaths = CreateWaypoints("EnemyWayPoint", "EnemyPoint", _enemyPathList);

        if (_bossPathList != null)
            _levelInfo.BossPaths = CreateWaypoints("BossWayPoint", "BossPoint", _bossPathList);
    }

    void Start()
    {
        if (OnLevelLoadedEvent != null)
            OnLevelLoadedEvent(_levelInfo);
    }

    #region CREATE_METHOD
    private void CreateMap()
    {
        _gridPrefab.GetComponent<Renderer>().sharedMaterial = _levelInfo.LevelLocation.GridMat;

        if (_levelInfo.LevelLocation.GroundTerrain)
            Instantiate(_levelInfo.LevelLocation.GroundTerrain);

        if (_levelInfo.LevelLocation.Skybox)
            RenderSettings.skybox = _levelInfo.LevelLocation.Skybox;

        _trans.position = Vector3.zero;
        int mapRow = _levelInfo.MapRow;
        int mapColumn = _levelInfo.MapColumn;

        for (int i = 0; i < mapRow; i++)
        {
            for (int k = 0; k < mapColumn; k++)
            {
                if (_mapBinary[i, k] == 1)
                {
                    Vector3 gridPos = Vector3.zero;
                    gridPos.x = (k - mapColumn / 2) * _gridSize;
                    gridPos.z = (i - mapRow / 2) * _gridSize;
                    gridPos.y = -0.5f;

                    GameObject gridGO = Instantiate(_gridPrefab, gridPos, Quaternion.identity) as GameObject;

                    gridGO.transform.parent = _trans;

                    gridGO.name = "Grid " + i + "-" + k;

                    GridMember gridMember = gridGO.GetComponent<GridMember>();
                    gridMember.GridIndex.SetIndex(i, k);
                }
            }
        }

        CreateBoundaries(mapRow, mapColumn);
    }

    private void CreateBoundaries(int mapRow, int mapCol)
    {
        _boundaryArray = new GameObject[4];

        Vector3 boundSize = new Vector3(mapRow * _gridSize + 2 * _boundaryToMapBorder, 4f, 0.5f);
        float offsetFromCenter = Mathf.Ceil(mapCol / 2) * _gridSize + 1 + _boundaryToMapBorder;

        _boundaryArray[0] = CreateBoundary("BoundaryTop", boundSize, Vector3.forward * offsetFromCenter, Quaternion.identity);

        _boundaryArray[1] = CreateBoundary("BoundaryBot", boundSize, Vector3.back * offsetFromCenter, Quaternion.identity);

        _boundaryArray[2] = CreateBoundary("BoundaryLeft", boundSize, Vector3.left * offsetFromCenter, Quaternion.Euler(0f, 90f, 0f));

        _boundaryArray[3] = CreateBoundary("BoundaryRight", boundSize, Vector3.right * offsetFromCenter, Quaternion.Euler(0f, 90f, 0f));

        for (int i = 0; i < _boundaryArray.Length; i++)
        {
            _boundaryArray[i].transform.parent = _trans;
        }
    }

    private GameObject CreateBoundary(string boundName, Vector3 size, Vector3 position, Quaternion rotation)
    {
        GameObject boundGO = new GameObject();
        boundGO.name = boundName;
        boundGO.layer = LayerMask.NameToLayer(_boundaryLayerName);
        position.y = size.y / 2;
        boundGO.transform.position = position;
        boundGO.transform.rotation = rotation;

        BoxCollider collider = boundGO.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = _boundryIsTrigger;
        return boundGO;
    }

    private PathManager CreateWaypoint(string containerName, string childName, Vector3[] waypoints)
    {
        GameObject wayPoint = new GameObject(containerName);
        wayPoint.transform.parent = _trans.parent;

        PathManager pathManager = wayPoint.AddComponent<PathManager>();
        Vector3[] enemyPath = waypoints;

        pathManager.waypoints = new Transform[enemyPath.Length];

        for (int i = 0; i < enemyPath.Length; i++)
        {
            GameObject pointGO = new GameObject(childName + i);
            pointGO.transform.position = enemyPath[i];
            pointGO.transform.parent = wayPoint.transform;

            pathManager.waypoints[i] = pointGO.transform;
        }

        return pathManager;
    }

    private PathManager[] CreateWaypoints(string containerName, string childName, List<Vector3[]> pathList)
    {
        int pathNumber = pathList.Count;

        PathManager[] pathManagerArray = new PathManager[pathNumber];

        for (int i = 0; i < pathNumber; i++)
        {
            pathManagerArray[i] = CreateWaypoint(containerName + i, childName, pathList[i]);
        }
        return pathManagerArray;
    }
    #endregion

    private void LoadMap(string mapName)
    {
        TextAsset mapXml = Resources.Load<TextAsset>(mapName);
        XmlDocument document = new XmlDocument();
        document.LoadXml(mapXml.text);

        XmlElement root = document.DocumentElement;

        int totalEnemyPoint = 0;
        int totalBossPoint = 0;
        int totalEnemyType = 0;

        if (root.Name == "Map")
        {
            // Map size
            _levelInfo.MapRow = Convert.ToInt32(root.Attributes["row"].Value);
            _levelInfo.MapColumn = Convert.ToInt32(root.Attributes["column"].Value);

            // Level location
            LevelLocationType locationType = (LevelLocationType)Enum.Parse(typeof(LevelLocationType), root.Attributes["location"].Value);

            for (int i = 0; i < _levelLocationArray.Length; i++)
            {
                if (_levelLocationArray[i].Location == locationType)
                {
                    _levelInfo.LevelLocation = _levelLocationArray[i];
                    break;
                }
            }

            // Camera size
            _levelInfo.CameraSize = (float)Convert.ToDouble(root.Attributes["camerasize"].Value);

            // Grid size
            _gridSize = Convert.ToInt32(root.Attributes["gridsize"].Value);

            // Starting cube number
            _levelInfo.StartingCubeNumber = Convert.ToInt32(root.Attributes["startingcubenumber"].Value);

            // Difficulties
            _levelInfo.Difficulty = (Difficulties)Enum.Parse(typeof(Difficulties), root.Attributes["difficulty"].Value);

            // Waypoint
            if (root.Attributes["enemywaypointnumber"] != null)
            {
                totalEnemyPoint = Convert.ToInt32(root.Attributes["enemywaypointnumber"].Value);
                _enemyPathList = new List<Vector3[]>(totalEnemyPoint);
            }

            if (root.Attributes["bosswaypointnumber"] != null)
            {
                totalBossPoint = Convert.ToInt32(root.Attributes["bosswaypointnumber"].Value);
                _bossPathList = new List<Vector3[]>(totalBossPoint);
            }

            if (root.Attributes["totalenemytype"] != null)
            {
                totalEnemyType = Convert.ToInt32(root.Attributes["totalenemytype"].Value);
                _levelInfo.Enemies = new LoadedEnemy[totalEnemyType];
            }

            _mapBinary = new int[_levelInfo.MapRow, _levelInfo.MapColumn];

            foreach (XmlNode childNode in root.ChildNodes)
            {
                if (childNode.Name == "Enemy")
                {
                    string enemyName = Convert.ToString(childNode.Attributes["name"].Value);
                    int count = Convert.ToInt32(childNode.Attributes["count"].Value);
                    int index = Convert.ToInt32(childNode.Attributes["index"].Value);

                    int[] pathIndexs = new int[Convert.ToInt32(childNode.Attributes["pathnumber"].Value)];

                    for (int i = 0; i < pathIndexs.Length; i++)
                    {
                        pathIndexs[i] = Convert.ToInt32(childNode.Attributes["pathindex" + i].Value);
                    }

                    bool isBoss = Convert.ToBoolean(childNode.Attributes["isboss"].Value);

                    for (int i = 0; i < _allEnemyInfo.Length; i++)
                    {
                        if (_allEnemyInfo[i].Prefab.name == enemyName)
                        {
                            LoadedEnemy enemy;
                            enemy.Count = count;
                            enemy.Prefab = _allEnemyInfo[i].Prefab;
                            enemy.IsBoss = isBoss;
                            enemy.PathIndexs = pathIndexs;
                            _levelInfo.Enemies[index] = enemy;
                            break;
                        }
                    }
                }
                else if (childNode.Name == "BossWaypoint")
                {
                    int pathIndex = Convert.ToInt32(childNode.Attributes["pathindex"].Value);
                    int length = Convert.ToInt32(childNode.Attributes["length"].Value);

                    _bossPathList.Add(new Vector3[length]);

                    foreach (XmlNode waypoint in childNode)
                    {
                        int index = Convert.ToInt32(waypoint.Attributes["index"].Value);
                        float x = (float)Convert.ToDouble(waypoint.Attributes["x"].Value);
                        float y = (float)Convert.ToDouble(waypoint.Attributes["y"].Value);
                        float z = (float)Convert.ToDouble(waypoint.Attributes["z"].Value);

                        (_bossPathList[pathIndex])[index] = new Vector3(x, y, z);
                    }
                }
                else if (childNode.Name == "EnemyWaypoint")
                {
                    int pathIndex = Convert.ToInt32(childNode.Attributes["pathindex"].Value);
                    int length = Convert.ToInt32(childNode.Attributes["length"].Value);

                    _enemyPathList.Add(new Vector3[length]);

                    foreach (XmlNode waypoint in childNode)
                    {
                        int index = Convert.ToInt32(waypoint.Attributes["index"].Value);
                        float x = (float)Convert.ToDouble(waypoint.Attributes["x"].Value);
                        float y = (float)Convert.ToDouble(waypoint.Attributes["y"].Value);
                        float z = (float)Convert.ToDouble(waypoint.Attributes["z"].Value);

                        (_enemyPathList[pathIndex])[index] = new Vector3(x, y, z);
                    }
                }

                else if (childNode.Name == "StarLevel")
                {
                    int[] starLevel = new int[4];
                    starLevel[0] = Convert.ToInt32(childNode.Attributes["starZero"].Value);
                    starLevel[1] = Convert.ToInt32(childNode.Attributes["starOne"].Value);
                    starLevel[2] = Convert.ToInt32(childNode.Attributes["starTwo"].Value);
                    starLevel[3] = Convert.ToInt32(childNode.Attributes["starThree"].Value);

                    _levelInfo.StarLevelConditions = starLevel;
                }

                else if (childNode.Name == "MapBoundary")
                {
                    // Boundaries
                    _boundaryLayerName = Convert.ToString(childNode.Attributes["boundaryLayerName"].Value);
                    _boundryIsTrigger = Convert.ToBoolean(childNode.Attributes["boundaryIsTrigger"].Value);
                    _boundaryToMapBorder = Convert.ToInt32(childNode.Attributes["boundaryToMapBorder"].Value);
                }

                else if (childNode.Name == "GameMode")
                {
                    // Game type
                    _levelInfo.GameType = (GameType)Enum.Parse(typeof(GameType), childNode.Attributes["gametype"].Value);

                    if (_levelInfo.GameType == GameType.TIME_ATTACK)
                    {
                        _levelInfo.TimeAttackLimit = Convert.ToInt32(childNode.Attributes["timelimit"].Value);
                    }
                }

                else if (childNode.Name == "InitPlayerPos")
                {
                    _levelInfo.PlayerInitPosition = new Index();
                    _levelInfo.PlayerInitPosition.Row = Convert.ToInt32(childNode.Attributes["row"].Value);
                    _levelInfo.PlayerInitPosition.Column = Convert.ToInt32(childNode.Attributes["column"].Value);
                }

                else if (childNode.Name == "Grid")
                {
                    int row = Convert.ToInt32(childNode.Attributes["x"].Value);
                    int col = Convert.ToInt32(childNode.Attributes["y"].Value);

                    int gridType = Convert.ToInt32(childNode.Attributes["type"].Value);

                    _mapBinary[row, col] = gridType;
                }
            }
        }
    }
}
