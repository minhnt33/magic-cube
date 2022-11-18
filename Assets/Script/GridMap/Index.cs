using UnityEngine;
using System.Collections;

[System.Serializable]
public class Index : System.Object
{
    [SerializeField]
    private int row;
    [SerializeField]
    private int column;

    public int Row { set { row = value; } get { return row; } }
    public int Column { set { column = value; } get { return column; } }

    public void SetIndex(int row, int col)
    {
        Row = row;
        Column = col;
    }

    public void SetIndex(Index other)
    {
        row = other.row;
        column = other.column;
    }

    public bool CompareIndex(Index other)
    {
        return (Row == other.Row && Column == other.Column) ? true : false;
    }

    public Index()
    {
        row = 0;
        column = 0;
    }

    public Index(int row, int col)
    {
        Row = row;
        Column = col;
    }

    public Index(Index index)
    {
        Row = index.Row;
        Column = index.Column;
    }
}
