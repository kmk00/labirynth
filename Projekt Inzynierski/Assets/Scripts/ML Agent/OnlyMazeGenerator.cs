using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private int _mazeWidth;
    [SerializeField]
    private int _mazeHeight;
    [SerializeField]
    private GameObject _player;


    private MazeCell[,] _mazeGrid;
    

    void Start()
    {

        Vector2Int[] corners = new Vector2Int[]{
            new Vector2Int(0, 0),
            new Vector2Int(_mazeWidth - 1, 0),
            new Vector2Int(0, _mazeHeight - 1),
            new Vector2Int(_mazeWidth - 1, _mazeHeight - 1)
        };

        Vector2Int selectedCorner = corners[Random.Range(0, corners.Length)]; // Destination Corner
        Vector2Int oppositeCorner = new Vector2Int(_mazeWidth - 1 - selectedCorner.x, _mazeHeight - 1 - selectedCorner.y); //Player Spawn Corner


        GenerateGrid(selectedCorner, oppositeCorner);

        GenerateMaze(null, _mazeGrid[selectedCorner.x, selectedCorner.y]);
        //InstantiatePlayer(oppositeCorner);
    }

    private void InstantiatePlayer(Vector2Int oppositeCorner)
    {
        Instantiate(_player, new Vector3(oppositeCorner.x, 1, oppositeCorner.y), Quaternion.identity);
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
                    cell = Instantiate(_endCell, new Vector3(i, 0, j), Quaternion.identity);
                else if (i == oppositeCorner.x && j == oppositeCorner.y)
                    cell = Instantiate(_startCell, new Vector3(i, 0, j), Quaternion.identity);
                else
                    cell = Instantiate(_cellPref, new Vector3(i, 0, j), Quaternion.identity);

                _mazeGrid[i, j] = cell;


                if (i == 0 || i == _mazeWidth - 1 || j == 0 || j == _mazeHeight - 1)
                {
                    //Outsidewall rozwiazuje problem z triggerem colliderow sciany 
                    TagChildObjects(cell,i,j);
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
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeHeight)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
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

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRight();
            currentCell.ClearLeft();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeft();
            currentCell.ClearRight();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFront();
            currentCell.ClearBack();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBack();
            currentCell.ClearFront();
            return;
        }
    }

}
