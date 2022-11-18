using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
using SWS;

public class LevelGenerator : MonoBehaviour
{
#if UNITY_EDITOR

    #region VAR
    [SerializeField]
    private GameObject _gridPrefab;
    [SerializeField]
    private float _gridSize;
    [SerializeField]
    private float _cameraOrthographicSize;
    [SerializeField]
    private int _sizeRow;
    [SerializeField]
    private int _sizeCol;
    [SerializeField]
    private Vector3 _startingPosition;
    [SerializeField]
    private bool _createBoundaries;
    [SerializeField]
    private string _boundLayerName;
    [SerializeField]
    private float _distanceToMapBorder;
    [SerializeField]
    private bool _isTrigger;
    [SerializeField]
    private string _directoryPath = "";
    [SerializeField]
    private string _fileName = "";
    [SerializeField]
    private GameType _gameMode;
    [SerializeField]
    private Vector2 _playerInitPos;
    [SerializeField]
    private int _startingCubeNumber;
    [SerializeField]
    private Difficulties _difficulty;
    [SerializeField]
    private LevelLocationType _levelLocation;
    [SerializeField]
    private float _timeLimit;
    [SerializeField]
    private int _starLevel0;
    [SerializeField]
    private int _starLevel1;
    [SerializeField]
    private int _starLevel2;
    [SerializeField]
    private int _starLevel3;
    [SerializeField]
    private List<PathManager> _enemyPathManager;
    [SerializeField]
    private List<PathManager> _bossPathManager;
    [SerializeField]
    private EnemyInformation[] _enemyInfo;

    private Transform _trans;
    private GridMember[,] _gridArray;
    private GameObject[] _boundaryArray;
    private Index _mapSize;

    public int SizeRow { get { return _sizeRow; } }
    public int SizeColumn { get { return _sizeCol; } }
    #endregion

    #region OTHER_METHOD
    private void InitGenerator()
    {
        _trans = transform;
        _gridArray = new GridMember[_sizeRow, _sizeCol];
        _mapSize = new Index(_sizeRow, _sizeCol);
        //OrthographicSize = GetSuitableOrthographicSize(_sizeCol);
    }

    public void CreateGrid()
    {
        InitGenerator();
        _trans.position = Vector3.zero;

        for (int i = 0; i < _sizeRow; i++)
        {
            for (int k = 0; k < _sizeCol; k++)
            {
                Vector3 gridPos = Vector3.zero;
                gridPos.x = (k - _mapSize.Column / 2) * _gridSize;
                gridPos.z = (i - _mapSize.Row / 2) * _gridSize;
                gridPos.y = -_gridSize / 2;

                GameObject gridGO = Instantiate(_gridPrefab, gridPos, Quaternion.identity) as GameObject;
                gridGO.transform.parent = _trans;

                gridGO.name = "Grid " + i + "-" + k;

                GridMember gridMember = gridGO.GetComponent<GridMember>();
                gridMember.GridIndex.SetIndex(i, k);
                _gridArray[i, k] = gridMember;
            }
        }

        AlignMapToStartingPosition();

        if (_createBoundaries)
            CreateBoundaries();
    }

    public void DeleteMap()
    {
        foreach (Transform child in transform)
        {
            SafeDestroyerUtils.SafeDestroyGameObject<Transform>(child);
        }

        _gridArray = null;
        _mapSize = null;
        _boundaryArray = null;
    }

    private void AlignMapToStartingPosition()
    {
        Vector3 tmpPos = _trans.position;
        tmpPos.z = tmpPos.z + _startingPosition.z;
        tmpPos.x = tmpPos.x + _startingPosition.x;
        tmpPos.y = tmpPos.y + _startingPosition.y;

        _trans.position = tmpPos;
    }

    private void CreateBoundaries()
    {
        _boundaryArray = new GameObject[4];

        Vector3 boundSizeLR = new Vector3(_mapSize.Row * _gridSize, 4f, 0.5f);
        Vector3 boundSizeTB = new Vector3(_mapSize.Column * _gridSize, 4f, 0.5f);
        float offsetFromCenterLR = Mathf.Ceil(_mapSize.Column / 2) * _gridSize + 1 + _distanceToMapBorder;
        float offsetFromCenterTB = Mathf.Ceil(_mapSize.Row / 2) * _gridSize + 1 + _distanceToMapBorder;

        _boundaryArray[0] = CreateBoundary("BoundaryTop", boundSizeTB, Vector3.forward * offsetFromCenterTB + _startingPosition, Quaternion.identity);

        _boundaryArray[1] = CreateBoundary("BoundaryBot", boundSizeTB, Vector3.back * offsetFromCenterTB + _startingPosition, Quaternion.identity);

        _boundaryArray[2] = CreateBoundary("BoundaryLeft", boundSizeLR, Vector3.left * offsetFromCenterLR + _startingPosition, Quaternion.Euler(0f, 90f, 0f));

        _boundaryArray[3] = CreateBoundary("BoundaryRight", boundSizeLR, Vector3.right * offsetFromCenterLR + _startingPosition, Quaternion.Euler(0f, 90f, 0f));

        for (int i = 0; i < _boundaryArray.Length; i++)
        {
            _boundaryArray[i].transform.parent = _trans;
        }
    }

    private GameObject CreateBoundary(string boundName, Vector3 size, Vector3 position, Quaternion rotation)
    {
        GameObject boundGO = new GameObject();
        boundGO.name = boundName;
        boundGO.layer = LayerMask.NameToLayer(_boundLayerName);
        position.y = size.y / 2;
        boundGO.transform.position = position;
        boundGO.transform.rotation = rotation;

        BoxCollider collider = boundGO.AddComponent<BoxCollider>();
        collider.size = size;
        collider.isTrigger = _isTrigger;
        return boundGO;
    }

    private float GetSuitableOrthographicSize(float mapSize)
    {
        if (mapSize == 5f)
            return 4f;
        else if (mapSize == 6f || mapSize == 7f)
            return 5f;
        else if (mapSize == 8f || mapSize == 9f)
            return 5.5f;

        return 0f;
    }
    #endregion

    public void ExportMap()
    {
        string path = _directoryPath + "/" + _fileName + ".xml";
        XmlWriter xmlWriter = XmlWriter.Create(path);

        xmlWriter.WriteStartDocument();

        // Map general properties
        xmlWriter.WriteStartElement("Map");
        xmlWriter.WriteAttributeString("camerasize", Convert.ToString(_cameraOrthographicSize));
        xmlWriter.WriteAttributeString("row", Convert.ToString(_sizeRow));
        xmlWriter.WriteAttributeString("column", Convert.ToString(_sizeCol));
        xmlWriter.WriteAttributeString("location", Convert.ToString(_levelLocation));
        xmlWriter.WriteAttributeString("difficulty", Convert.ToString(_difficulty));
        xmlWriter.WriteAttributeString("gridsize", Convert.ToString(_gridSize));
        xmlWriter.WriteAttributeString("startingcubenumber", Convert.ToString(_startingCubeNumber));

        if (_enemyPathManager != null)
            xmlWriter.WriteAttributeString("enemywaypointnumber", Convert.ToString(_enemyPathManager.Count));
        if (_bossPathManager != null)
            xmlWriter.WriteAttributeString("bosswaypointnumber", Convert.ToString(_bossPathManager.Count));
        if (_enemyInfo != null)
            xmlWriter.WriteAttributeString("totalenemytype", Convert.ToString(_enemyInfo.Length));

        // Boundaries
        if (_createBoundaries)
        {
            xmlWriter.WriteStartElement("MapBoundary");
            xmlWriter.WriteAttributeString("boundaryLayerName", Convert.ToString(_boundLayerName));
            xmlWriter.WriteAttributeString("boundaryIsTrigger", Convert.ToString(_isTrigger));
            xmlWriter.WriteAttributeString("boundaryToMapBorder", Convert.ToString(_distanceToMapBorder));
            xmlWriter.WriteEndElement();
        }

        // Star level
        xmlWriter.WriteStartElement("StarLevel");
        xmlWriter.WriteAttributeString("starZero", Convert.ToString(_starLevel0));
        xmlWriter.WriteAttributeString("starOne", Convert.ToString(_starLevel1));
        xmlWriter.WriteAttributeString("starTwo", Convert.ToString(_starLevel2));
        xmlWriter.WriteAttributeString("starThree", Convert.ToString(_starLevel3));
        xmlWriter.WriteEndElement();

        // Game mode
        xmlWriter.WriteStartElement("GameMode");
        xmlWriter.WriteAttributeString("gametype", Convert.ToString(_gameMode));
        if (_gameMode == GameType.TIME_ATTACK)
        {
            xmlWriter.WriteAttributeString("timelimit", Convert.ToString(_timeLimit));
        }
        xmlWriter.WriteEndElement();

        // Player init pos
        xmlWriter.WriteStartElement("InitPlayerPos");
        xmlWriter.WriteAttributeString("row", Convert.ToString(_playerInitPos.x));
        xmlWriter.WriteAttributeString("column", Convert.ToString(_playerInitPos.y));
        xmlWriter.WriteEndElement();

        for (int k = 0; k < _enemyInfo.Length; k++)
        {
            GameObject enemyPrefab = _enemyInfo[k].Prefab;
            xmlWriter.WriteStartElement("Enemy");
            xmlWriter.WriteAttributeString("name", Convert.ToString(enemyPrefab.name));
            xmlWriter.WriteAttributeString("index", Convert.ToString(k));
            xmlWriter.WriteAttributeString("count", Convert.ToString(_enemyInfo[k].Count));
            xmlWriter.WriteAttributeString("isboss", Convert.ToString(_enemyInfo[k].IsBoss));
            xmlWriter.WriteAttributeString("pathnumber", Convert.ToString(_enemyInfo[k].PathIndexs.Length));

            for (int i = 0; i < _enemyInfo[k].PathIndexs.Length; i++)
            {
                xmlWriter.WriteAttributeString("pathindex" + i, Convert.ToString(_enemyInfo[k].PathIndexs[i]));
            }

            xmlWriter.WriteEndElement();
        }

        // Enemy paths
        if (_enemyPathManager != null)
        {
            for (int k = 0; k < _enemyPathManager.Count; k++)
            {
                Vector3[] wayPoints = _enemyPathManager[k].GetPathPoints();
                xmlWriter.WriteStartElement("EnemyWaypoint");
                xmlWriter.WriteAttributeString("pathindex", Convert.ToString(k));
                xmlWriter.WriteAttributeString("length", Convert.ToString(wayPoints.Length));
                for (int i = 0; i < wayPoints.Length; i++)
                {
                    xmlWriter.WriteStartElement("Waypoint");
                    xmlWriter.WriteAttributeString("index", Convert.ToString(i));
                    xmlWriter.WriteAttributeString("x", Convert.ToString(wayPoints[i].x));
                    xmlWriter.WriteAttributeString("y", Convert.ToString(wayPoints[i].y));
                    xmlWriter.WriteAttributeString("z", Convert.ToString(wayPoints[i].z));
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }

        // Boss paths
        if (_bossPathManager != null)
        {
            for (int k = 0; k < _bossPathManager.Count; k++)
            {
                Vector3[] wayPoints = _bossPathManager[k].GetPathPoints();
                xmlWriter.WriteStartElement("BossWaypoint");
                xmlWriter.WriteAttributeString("pathindex", Convert.ToString(k));
                xmlWriter.WriteAttributeString("length", Convert.ToString(wayPoints.Length));
                for (int i = 0; i < wayPoints.Length; i++)
                {
                    xmlWriter.WriteStartElement("Waypoint");
                    xmlWriter.WriteAttributeString("index", Convert.ToString(i));
                    xmlWriter.WriteAttributeString("x", Convert.ToString(wayPoints[i].x));
                    xmlWriter.WriteAttributeString("y", Convert.ToString(wayPoints[i].y));
                    xmlWriter.WriteAttributeString("z", Convert.ToString(wayPoints[i].z));
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }

        GridMember[,] initGridMap = new GridMember[_sizeRow, _sizeCol];

        GridMember[] gridMap = GetComponentsInChildren<GridMember>();

        // Fill init array with real grid member
        for (int i = 0; i < gridMap.Length; i++)
        {
            Index index = gridMap[i].GridIndex;
            initGridMap[index.Row, index.Column] = gridMap[i];
        }

        // Write all map to xml
        for (int r = 0; r < _sizeRow; r++)
        {
            for (int c = 0; c < _sizeCol; c++)
            {
                xmlWriter.WriteStartElement("Grid");

                xmlWriter.WriteAttributeString("x", Convert.ToString(r));
                xmlWriter.WriteAttributeString("y", Convert.ToString(c));

                if (initGridMap[r, c] != null)
                    xmlWriter.WriteAttributeString("type", "1");
                else
                    xmlWriter.WriteAttributeString("type", "0");

                xmlWriter.WriteEndElement();
            }
        }

        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
    }

#endif
}

[System.Serializable]
public class EnemyInformation : System.Object
{
    public GameObject Prefab;
    public int Count;
    public bool IsBoss;
    public int[] PathIndexs;
}
