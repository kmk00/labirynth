using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MazeGeneratorBase : MonoBehaviour
{

    [SerializeField]
    protected MazeCell _cellPref;
    [SerializeField]
    protected MazeCell _startCell;
    [SerializeField]
    protected MazeCell _endCell;
    [SerializeField]
    protected int _minimum;

    protected int _mazeWidth;
    protected int _mazeHeight;
    protected MazeCell[,] _mazeGrid;

    protected abstract void GenerateGrid(Vector2Int selectedCorner, Vector2Int oppositeCorner);

    protected void TagChildObjects(MazeCell parent, int i, int j)
    {
        foreach (Transform child in parent.transform)
        {
            if (i == 0 && child.gameObject.name == "Left Wall")
                child.gameObject.tag = "OutsideWall";
            if (i == 0 && child.gameObject.name == "Front Wall")
                child.gameObject.tag = "OutsideWall";
            if (j == 0 && child.gameObject.name == "Back Wall")
                child.gameObject.tag = "OutsideWall";
            if (i == _mazeWidth - 1 && child.gameObject.name == "Right Wall")
                child.gameObject.tag = "OutsideWall";
            if (j == _mazeHeight - 1 && child.gameObject.name == "Front Wall")
                child.gameObject.tag = "OutsideWall";
        }
    }

    protected void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
                GenerateMaze(currentCell, nextCell);

        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 20)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.localPosition.x + _mazeWidth / 2);
        int z = Mathf.RoundToInt(currentCell.transform.localPosition.z + _mazeHeight / 2);

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (!cellToRight.IsVisited)
                yield return cellToRight;
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (!cellToLeft.IsVisited)
                yield return cellToLeft;
        }

        if (z + 1 < _mazeHeight)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (!cellToFront.IsVisited)
                yield return cellToFront;
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (!cellToBack.IsVisited)
                yield return cellToBack;
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
            return;

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
