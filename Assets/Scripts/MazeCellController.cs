using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellController : MonoBehaviour
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

    public Boolean isVisited { get; private set; };

    public void Visit()
    {
        isVisited = true;
        unvisitedBlock.SetActive(false);
    }
}
