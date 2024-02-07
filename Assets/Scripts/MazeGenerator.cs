using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell mazeCellPrefab;

    [SerializeField]
    private int mazeWidth;

    [SerializeField]
    private int mazeDepth;

    private MazeCell[,]  mazeGrid;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for(int i = 0; i < mazeWidth; i++)
        {
            for(int j = 0;  j < mazeDepth; j++)
            {
                mazeGrid[i, j] = Instantiate(mazeCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, mazeGrid[0, 0]);
    }

    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.025f);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        }while(nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        //Check the cell to the right
        if(x + 1 < mazeWidth)
        {
            var rightCell = mazeGrid[x + 1, z];
            if(rightCell.isVisited == false)
            {
                yield return rightCell;
            }
        }

        //Check the cell to the left
        if (x - 1 >= 0)
        {
            var leftCell = mazeGrid[x - 1, z];
            if (leftCell.isVisited == false)
            {
                yield return leftCell;
            }
        }

        //Check the cell to the front
        if (z - 1 >= 0)
        {
            var frontCell = mazeGrid[x, z - 1];
            if (frontCell.isVisited == false)
            {
                yield return frontCell;
            }
        }

        //Check the cell to the back
        if (z + 1 < mazeDepth)
        {
            var backCell = mazeGrid[x, z + 1];
            if (backCell.isVisited == false)
            {
                yield return backCell;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        //When we start, previous cell doesn't exist
        if(previousCell == null)
        {
            return;
        }

        //If the previous cell is on the left, clear current left wall and previous right wall
        if(previousCell.transform.position.x < currentCell.transform.position.x)
        {
            currentCell.ClearLeftWall();
            previousCell.ClearRightWall();
            return;
        }

        //If the previous cell is on the right, clear current right wall and previous left wall
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            currentCell.ClearRightWall();
            previousCell.ClearLeftWall();
            return;
        }

        //If the previous cell is on the front, clear current front wall and previous back wall
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            currentCell.ClearBackWall();
            previousCell.ClearFrontWall();
            return;
        }

        //If the previous cell is on the back, clear current back wall and previous front wall
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            currentCell.ClearFrontWall();
            previousCell.ClearBackWall();
            return;
        }
    }
}