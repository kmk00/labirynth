using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    [SerializeField]
    private MazeCell _cellPref;
    [SerializeField]
    private MazeCell _startCell;
    [SerializeField]
    private MazeCell _endCell;
    private int _mazeWidth;
    private int _mazeHeight;
    [SerializeField]
    private GameObject _player;


    private MazeCell[,] _mazeGrid;
    

    void Start()
    {
        _mazeWidth = SharedData.X;
        _mazeHeight = SharedData.Y;
        GenerateGrid();
        GenerateMaze(null, _mazeGrid[0, 0]);
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        Instantiate(_player, new Vector3(_mazeHeight-1, 1, _mazeWidth-1), Quaternion.identity);
    }

    private void GenerateGrid()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeHeight];
        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeHeight; j++)
            {
                if(i== 0 && j == 0) _mazeGrid[i, j] = Instantiate(_endCell, new Vector3(i, 0, j), Quaternion.identity); // End cell wybieram jakio pierwsze, zeby miec pewnosc ze niszczymy tylko jedna sciane na tym kafelku
                else if (i == _mazeWidth - 1 && j == _mazeHeight - 1) _mazeGrid[i, j] = Instantiate(_startCell, new Vector3(i, 0, j), Quaternion.identity);
                else _mazeGrid[i, j] = Instantiate(_cellPref, new Vector3(i, 0, j), Quaternion.identity);
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
