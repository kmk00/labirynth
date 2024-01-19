using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlyMazeGenerator : MazeGeneratorBase
{
    [SerializeField]
    private GameObject AI_Algorithm;

    private GameObject environmentParent;



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

        if(environmentParent.name == "AIMaze")
        {
            Vector3 aiPosition = new Vector3(oppositeCorner.x - _mazeWidth / 2, 20.4f, oppositeCorner.y - _mazeHeight / 2);
            Instantiate(AI_Algorithm, aiPosition, Quaternion.identity, environmentParent.transform);
        }


    }


    protected override void GenerateGrid(Vector2Int selectedCorner, Vector2Int oppositeCorner)
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
}
