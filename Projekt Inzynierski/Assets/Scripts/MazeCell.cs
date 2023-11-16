using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallRight;

    [SerializeField]
    private GameObject _wallLeft;

    [SerializeField]
    private GameObject _wallFront;

    [SerializeField]
    private GameObject _wallBack;

    [SerializeField]
    private GameObject _unvisitedCell;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedCell.SetActive(false);
    }

    public void ClearLeft()
    {
        _wallLeft.SetActive(false);
    }

    public void ClearBack()
    {
        _wallBack.SetActive(false);
    }

    public void ClearFront()
    {
        _wallFront.SetActive(false);
    }

    public void ClearRight()
    {
        _wallRight.SetActive(false);
    }
}
