using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject leftWall;

    [SerializeField]
    private GameObject rightWall;

    [SerializeField]
    private GameObject frontWall;

    [SerializeField]
    private GameObject backWall;

    [SerializeField]
    private GameObject unvisitedBlock;

    [SerializeField]
    private Material pathMaterial;

    [SerializeField]
    private GameObject floorGraphics;

    public bool isVisited { get; private set; }

    /// <summary>
    /// Disables the unvisited block to visually show a cell that has been visited
    /// </summary>
    public void Visit()
    {
        isVisited = true;
        unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        backWall.SetActive(false);
    }

    public bool IsActiveLeftWall()
    {
        return leftWall.activeInHierarchy;
    }

    public bool IsActiveRightWall()
    {
        return rightWall.activeInHierarchy;
    }

    public bool IsActiveFrontWall()
    {
        return frontWall.activeInHierarchy;
    }

    public bool IsActiveBackWall()
    {
        return backWall.activeInHierarchy;
    }

    public void ActivateWalls()
    {
        leftWall.SetActive(true);
        rightWall.SetActive(true);
        frontWall.SetActive(true);
        backWall.SetActive(true);

        isVisited = false;
        unvisitedBlock.SetActive(true);
    }

    public void BoundaryLeft()
    {
        leftWall.tag = "Boundary";
    }

    public void BoundaryBack()
    {
        backWall.tag = "Boundary";
    }

    public void BoundaryFront()
    {
        frontWall.tag = "Boundary";
    }

    public void BoundaryRight()
    {
        rightWall.tag = "Boundary";
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    public void ChangeFloorToPath()
    {
        floorGraphics.GetComponent<Renderer>().material = pathMaterial;
    }
}