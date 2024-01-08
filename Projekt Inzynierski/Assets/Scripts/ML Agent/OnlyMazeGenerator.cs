using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlyMazeGenerator : MonoBehaviour
{

    [SerializeField]
    private MazeCell _cellPref;
    [SerializeField]
    private MazeCell _startCell;
    [SerializeField]
    private MazeCell _endCell;
    [SerializeField]
    private int _minimum;

    private int _mazeWidth;
    private int _mazeHeight;

    private GameObject environmentParent;

    private MazeCell[,] _mazeGrid;
    

    void Start()
    {

        if(SharedData.IsMinimumOn)
        {
            _minimum = 3;
            _mazeWidth = Random.Range(_minimum, SharedData.X);
            _mazeHeight = Random.Range(_minimum, SharedData.Y);
        }
        else
        {
            _mazeWidth = SharedData.X;
            _mazeHeight = SharedData.Y;
            Debug.Log("Sztywno");
        }
        Debug.Log("x: "+ _mazeWidth + ",y: " + _mazeHeight);
        Vector2Int[] corners = new Vector2Int[]{
            new Vector2Int(0, 0),
            new Vector2Int(_mazeWidth - 1, 0),
            new Vector2Int(0, _mazeHeight - 1),
            new Vector2Int(_mazeWidth - 1, _mazeHeight - 1)
        };

        Vector2Int selectedCorner = corners[Random.Range(0, corners.Length)]; // Destination Corner
        Vector2Int oppositeCorner = new Vector2Int(_mazeWidth - 1 - selectedCorner.x, _mazeHeight - 1 - selectedCorner.y); //Player Spawn Corner

        environmentParent = transform.parent.gameObject;

        GenerateGrid(selectedCorner, oppositeCorner);

        if (_mazeGrid[selectedCorner.x, selectedCorner.y] != null)
        {
            GenerateMaze(null, _mazeGrid[selectedCorner.x, selectedCorner.y]);
        }
        else
        {
            Debug.LogError("Selected corner cell is null. Maze generation failed.");
        }

    }


    private void GenerateGrid(Vector2Int selectedCorner, Vector2Int oppositeCorner)
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];


        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                MazeCell cell;
                if (i == selectedCorner.x && j == selectedCorner.y)
                    cell = Instantiate(_endCell, new Vector3(i, 0, j), Quaternion.identity, environmentParent.transform);
                else if (i == oppositeCorner.x && j == oppositeCorner.y)
                    cell = Instantiate(_startCell, new Vector3(i, 0, j), Quaternion.identity, environmentParent.transform);
                else
                    cell = Instantiate(_cellPref, new Vector3(i, 0, j), Quaternion.identity, environmentParent.transform);



                _mazeGrid[i, j] = cell;

                cell.transform.localPosition = new Vector3(i - _mazeWidth / 2, 0, j - _mazeHeight / 2);

                if (i == 0 || i == _mazeWidth - 1 || j == 0 || j == _mazeHeight - 1)
                {
                   TagChildObjects(cell, i, j);
                }
            }
        }
    }

    private void TagChildObjects(MazeCell parent,int i, int j)
    {
        foreach (Transform child in parent.transform)
        {
            if (i == 0 && child.gameObject.name == "Left Wall")
            {
                child.gameObject.tag = "OutsideWall";
            }
            if (i == 0 && child.gameObject.name == "Front Wall")
            {
                child.gameObject.tag = "OutsideWall";
            }
            if (j == 0 && child.gameObject.name == "Back Wall")
            {
                child.gameObject.tag = "OutsideWall";
            }
            if (i == _mazeWidth - 1 && child.gameObject.name == "Right Wall")
            {
                child.gameObject.tag = "OutsideWall";
            }
            if (j == _mazeHeight - 1 && child.gameObject.name == "Front Wall")
            {
                child.gameObject.tag = "OutsideWall";
            }
        }
    }


    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 20)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        // Adjusting for the offset used in GenerateGrid
        int x = Mathf.RoundToInt(currentCell.transform.localPosition.x + _mazeWidth / 2);
        int z = Mathf.RoundToInt(currentCell.transform.localPosition.z + _mazeHeight / 2);

        // Check each direction, ensuring indices are within the bounds of the grid
        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (!cellToRight.IsVisited)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (!cellToLeft.IsVisited)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeHeight)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (!cellToFront.IsVisited)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (!cellToBack.IsVisited)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        Vector3 prevLocalPos = previousCell.transform.localPosition;
        Vector3 currLocalPos = currentCell.transform.localPosition;

        if (prevLocalPos.x < currLocalPos.x)
        {
            previousCell.ClearRight();
            currentCell.ClearLeft();
            return;
        }

        if (prevLocalPos.x > currLocalPos.x)
        {
            previousCell.ClearLeft();
            currentCell.ClearRight();
            return;
        }

        if (prevLocalPos.z < currLocalPos.z)
        {
            previousCell.ClearFront();
            currentCell.ClearBack();
            return;
        }

        if (prevLocalPos.z > currLocalPos.z)
        {
            previousCell.ClearBack();
            currentCell.ClearFront();
            return;
        }
    }
}
