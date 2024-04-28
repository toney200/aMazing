using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SingleplayerGenerator : MonoBehaviour
{
    public struct Pair
    {
        public int first, second;

        public Pair(int x, int y)
        {
            first = x;
            second = y;
        }
    }

    public struct CellForPath
    {
        MazeCell cell;
        public MazeCell parentCell;
        public double f, g, h;
    }

    [SerializeField]
    private MazeCell mazeCellPrefab;

    [SerializeField]
    public MazeCell mazeGoalPrefab;

    [SerializeField]
    private int mazeWidth;

    [SerializeField]
    private int mazeDepth;

    private MazeCell[,] mazeGrid;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject playersParent;

    [SerializeField]
    private int numberOfPlayers;

    public GameObject[] playerPrefabList;

    private int[,] cellValue;

    public TextMeshProUGUI blueScoreText;
    private int blueScore = 0;

    public static bool first = true;

    public GameObject music;
    public AudioSource goalSFX;
    public GameObject collectible;

    private int biggestValue;
    private int[] furthestCell;
    private MazeCell finish;

    // Start is called before the first frame update
    void Awake()
    {
        if (first)
        {
            PlayerPrefs.DeleteAll();
            first = false;
            music.SetActive(true);
        }
        else
        {
            blueScore = PlayerPrefs.GetInt("Blue Score", 0);
            blueScoreText.text = blueScore.ToString();
        }
    }

    void Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        cellValue = new int[mazeWidth, mazeDepth];
        furthestCell = new int[2];

        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                mazeGrid[i, j] = Instantiate(mazeCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
                cellValue[i, j] = 0;
                int random = Random.Range(0, 101);
                if(random % 7 == 0)
                {
                    mazeGrid[i, j].IceFloor();
                }
                if(random % 5 == 0)
                {
                    Instantiate(collectible, new Vector3(i, 0.35f, j), Quaternion.identity, gameObject.transform);
                }
            }
        }
        StartAtRandomEdgeCell(true);

        //Generating the goal that would take the player the longest to get to
        MazeCell goal = Instantiate(mazeGoalPrefab, new Vector3(furthestCell[0], 50, furthestCell[1]), Quaternion.identity, gameObject.transform);
        MazeCell toDestroy = mazeGrid[furthestCell[0], furthestCell[1]];
        CopyWalls(toDestroy, goal);
        goal.transform.position = new Vector3(Mathf.RoundToInt(furthestCell[0]), 0, Mathf.RoundToInt(furthestCell[1]));
        mazeGrid[furthestCell[0], furthestCell[1]] = goal;
        finish = goal;
        toDestroy.Remove();
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            FindPathToGoal(mazeGrid[Mathf.RoundToInt(playersParent.transform.GetChild(0).gameObject.transform.position.x), Mathf.RoundToInt(playersParent.transform.GetChild(0).gameObject.transform.position.z)], finish);
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
                cellValue[(int)nextCell.transform.position.x, (int)nextCell.transform.position.z] = cellValue[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z] + 1;
                if(cellValue[(int)nextCell.transform.position.x, (int)nextCell.transform.position.z] > biggestValue)
                {
                    biggestValue = cellValue[(int)nextCell.transform.position.x, (int)nextCell.transform.position.z];
                    furthestCell[0] = (int)nextCell.transform.position.x;
                    furthestCell[1] = (int)nextCell.transform.position.z;
                }
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    /// <summary>
    /// Randomly chooses an unvisited neighbouring cell
    /// </summary>
    /// <param name="currentCell"></param>
    /// <returns></returns>
    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    /// <summary>
    /// Returns all neighbouring cells that have not yet been visited.
    /// </summary>
    /// <param name="currentCell"></param>
    /// <returns></returns>
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        //Check the cell to the right
        if (x + 1 < mazeWidth)
        {
            var rightCell = mazeGrid[x + 1, z];
            if (rightCell.isVisited == false)
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

    /// <summary>
    /// Clears the walls of the two cells passed in order to connect them.
    /// </summary>
    /// <param name="previousCell"></param>
    /// <param name="currentCell"></param>
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        //When we start, previous cell doesn't exist
        if (previousCell == null)
        {
            return;
        }

        //If the previous cell is on the left, clear current left wall and previous right wall
        if (previousCell.transform.position.x < currentCell.transform.position.x)
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

    /// <summary>
    /// Chooses a random cell at one of the edges of the maze as a starting point for generating the maze.
    /// Players are also spawned in the maze relative to the chosen cell.
    /// </summary>
    /// <param name="isFirstRound"></param>
    private void StartAtRandomEdgeCell(bool isFirstRound)
    {
        int edgeChooser = Mathf.RoundToInt(Random.Range(1.0f, 2.0f));
        int xStartPoint, zStartPoint;

        switch (edgeChooser)
        {
            case 1:
                edgeChooser = Mathf.RoundToInt(Random.Range(1.0f, 2.0f));
                xStartPoint = edgeChooser % 2 == 0 ? 0 : mazeWidth - 1;
                zStartPoint = Mathf.RoundToInt(Random.Range(0.0f, (mazeDepth - 1)));

                cellValue[xStartPoint, zStartPoint] = 0;
                GenerateMaze(null, mazeGrid[xStartPoint, zStartPoint]);
                break;
            case 2:
                edgeChooser = Mathf.RoundToInt(Random.Range(1.0f, 2.0f));
                zStartPoint = edgeChooser % 2 == 0 ? 0 : mazeDepth - 1;
                xStartPoint = Mathf.RoundToInt(Random.Range(0.0f, (mazeWidth - 1)));

                cellValue[xStartPoint, zStartPoint] = 0;
                GenerateMaze(null, mazeGrid[xStartPoint, zStartPoint]);
                break;
            default:
                xStartPoint = 0;
                zStartPoint = 0;
                break;
        }

        if (isFirstRound)
        {
            SpawnPlayers(xStartPoint, zStartPoint);  //DO NOT PUT IN Start(), coordinates from this method are needed
        }
        else
        {
            NewPlayerSpawnPoints(xStartPoint, zStartPoint);
        }
    }

    /// <summary>
    /// Spawns players at the start of the game
    /// </summary>
    /// <param name="xStart"></param>
    /// <param name="zStart"></param>
    private void SpawnPlayers(int xStart, int zStart)
    {
        int edge = CheckEdge(xStart, zStart);
        int distance = GetDistance(xStart, zStart, edge);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i == 0)
            {
                Instantiate(playerPrefabList[i], new Vector3((float)xStart, 0.35f, (float)zStart), Quaternion.identity, playersParent.transform);
                if (edge == 3)
                {
                    edge = 0;
                }
                else
                {
                    edge++;
                }
            }
            else
            {
                switch (edge)
                {
                    //Front edge case
                    case 0:
                        Instantiate(playerPrefabList[i], new Vector3((float)distance, 0.35f, 0), Quaternion.identity, playersParent.transform);
                        edge++;
                        break;
                    //Left edge case
                    case 1:
                        Instantiate(playerPrefabList[i], new Vector3((float)(mazeWidth - 1), 0.35f, (float)distance), Quaternion.identity, playersParent.transform);
                        edge++;
                        break;
                    //Back edge case
                    case 2:
                        Instantiate(playerPrefabList[i], new Vector3((float)((mazeWidth - 1) - distance), 0.35f, (float)(mazeDepth - 1)), Quaternion.identity, playersParent.transform);
                        edge++;
                        break;
                    //Right edge case
                    case 3:
                        Instantiate(playerPrefabList[i], new Vector3(0, 0.35f, (float)((mazeDepth - 1) - distance)), Quaternion.identity, playersParent.transform);
                        edge = 0;
                        break;
                    //Failure
                    default:
                        Debug.Log("Error in calculation");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Checks which edge the cell with the given coordinates is on.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>Returns an integer representing an edge.</returns>
    private int CheckEdge(int x, int z)
    {
        //Checks if it's the front edge, denoted by 0
        if ((x >= 0 && x <= mazeWidth - 2) && z == 0)
        {
            return 0;
        }
        //Checks if it's the left edge, denoted by 1
        if ((z >= 0 && z <= mazeDepth - 2) && x == mazeWidth - 1)
        {
            return 1;
        }
        //Checks if it's the back edge, denoted by 2
        if ((x >= 1 && x <= mazeWidth - 1) && z == mazeDepth - 1)
        {
            return 2;
        }
        //Checks if it's the right edge, denoted by 3
        if ((z >= 1 && z <= mazeDepth - 1) && x == 0)
        {
            return 3;
        }

        //-1 indicates a failure, it should never come to this
        return -1;
    }

    /// <summary>
    /// Checks how far the beginning maze cell is from the origin of the edge it is a part of.
    /// Front origin (0, 0), Left origin (mazeWidth - 1, 0), Back origin (mazeWidth - 1, mazeDepth - 1), Right origin (0, mazeDepth - 1)
    /// </summary>
    /// <param name="xLocation"></param>
    /// <param name="zLocation"></param>
    /// <param name="edgeNumber"></param>
    /// <returns>The distance from the edge's origin</returns>
    private int GetDistance(int xLocation, int zLocation, int edgeNumber)
    {
        switch (edgeNumber)
        {
            case 0:
                return xLocation;
            case 1:
                return zLocation;
            case 2:
                return (mazeWidth - 1) - xLocation;
            case 3:
                return (mazeDepth - 1) - zLocation;
            default:
                Debug.Log("Cannot get edge distance");
                return -1;
        }
    }

    /// <summary>
    /// Starts a new round when called
    /// </summary>
    public void NewRound(int winner)
    {
        switch (winner)
        {
            case 1:
                blueScore += 10;
                blueScoreText.text = blueScore.ToString();
                PlayerPrefs.SetInt("Blue Score", blueScore);
                break;
            default:
                break;
        }

        if (blueScore >= 100)
        {
            first = true;
            SceneManager.LoadScene("Victory");
        }
        else
        {
            goalSFX.Play();
            DontDestroyOnLoad(goalSFX);
            DontDestroyOnLoad(music);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



        /*        foreach(MazeCell mc in mazeGrid)
                {
                    mc.ActivateWalls();
                }

                StartAtRandomEdgeCell(false);*/
    }

    public void IncreaseScore()
    {
        blueScore++;
        blueScoreText.text = blueScore.ToString();
        PlayerPrefs.SetInt("Blue Score", blueScore);
    }

    /// <summary>
    /// Generates new spawn points for players without the need to delete and instantiate new players
    /// </summary>
    /// <param name="xStart"></param>
    /// <param name="zStart"></param>
    private void NewPlayerSpawnPoints(int xStart, int zStart)
    {
        int edge = CheckEdge(xStart, zStart);
        int distance = GetDistance(xStart, zStart, edge);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (i == 0)
            {
                playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3((float)xStart, 0.35f, (float)zStart);
                if (edge == 3)
                {
                    edge = 0;
                }
                else
                {
                    edge++;
                }
            }
            else
            {
                switch (edge)
                {
                    //Front edge case
                    case 0:
                        playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3((float)distance, 0.35f, 0);
                        edge++;
                        break;
                    //Left edge case
                    case 1:
                        playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3((float)(mazeWidth - 1), 0.35f, (float)distance);
                        edge++;
                        break;
                    //Back edge case
                    case 2:
                        playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3((float)((mazeWidth - 1) - distance), 0.35f, (float)(mazeDepth - 1));
                        edge++;
                        break;
                    //Right edge case
                    case 3:
                        playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3(0, 0.35f, (float)((mazeDepth - 1) - distance));
                        edge = 0;
                        break;
                    //Failure
                    default:
                        Debug.Log("Error in calculation");
                        break;
                }
            }
        }
    }

    private void CopyWalls(MazeCell toBeReplaced, MazeCell goal)
    {
        if(toBeReplaced.IsActiveLeftWall() == false)
        {
            goal.ClearLeftWall();
        }

        if (toBeReplaced.IsActiveRightWall() == false)
        {
            goal.ClearRightWall();
        }

        if (toBeReplaced.IsActiveFrontWall() == false)
        {
            goal.ClearFrontWall();
        }

        if (toBeReplaced.IsActiveBackWall() == false)
        {
            goal.ClearBackWall();
        }
    }

    public void FindPathToGoal(MazeCell cellStart, MazeCell goal)
    {
        var connectedCells = GetConnectedCells(cellStart);
        bool[,] visitedList = new bool[mazeWidth, mazeDepth];
        CellForPath[,] cellInfo = new CellForPath[mazeWidth, mazeDepth];

        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                cellInfo[i, j].f = double.MaxValue;
                cellInfo[i, j].g = double.MaxValue;
                cellInfo[i, j].h = double.MaxValue;
                cellInfo[i, j].parentCell = null;
            }
        }

        int x = (int)cellStart.transform.position.x;
        int z = (int) cellStart.transform.position.z;
        cellInfo[x, z].f = 0.0;
        cellInfo[x, z].g = 0.0;
        cellInfo[x, z].h = 0.0;
        cellInfo[x, z].parentCell = cellStart;

        SortedSet<(double, Vector2)> openList = new SortedSet<(double,Vector2)>(Comparer<(double, Vector2)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));

        openList.Add((0.0, new Vector2(cellStart.transform.position.x, cellStart.transform.position.z)));

        while (openList.Count > 0)
        {
            (double f, Vector2 v) p = openList.Min;
            openList.Remove(p);

            x = (int) p.v.x;
            z = (int) p.v.y;
            visitedList[x, z] = true;
            MazeCell currentCell = mazeGrid[x, z];

            IEnumerable<MazeCell> neighbours = GetConnectedCells(mazeGrid[x, z]);
            foreach(MazeCell cell in neighbours)
            {
                int newX = (int) cell.transform.position.x;
                int newY = (int)cell.transform.position.z;
                mazeGrid[newX, newY].ChangeFloorToPath();
                if (newX == furthestCell[0] && newY == furthestCell[1])
                {
                    cellInfo[newX, newY].parentCell = currentCell;
                    TracePath(cellInfo, goal);
                    return;
                }

                if (!visitedList[newX, newY])
                {
                    double gNew = cellInfo[x, z].g + 1.0;
                    double hNew = CalculateHValue(newX, newY, goal);
                    double fNew = gNew + hNew;

                    if (cellInfo[newX, newY].f == double.MaxValue || cellInfo[newX, newY].f > fNew)
                    {
                        openList.Add((fNew, new Vector2(newX, newY)));

                        cellInfo[newX, newY].f = fNew;
                        cellInfo[newX, newY].g = gNew;
                        cellInfo[newX, newY].h = hNew;
                        cellInfo[newX, newY].parentCell = currentCell;
                    }
                }
            }
        }
    }

    private void TracePath(CellForPath[,] cellInfo, MazeCell goal)
    {
        Debug.Log("Tracing Path");
        int row = (int)goal.transform.position.x;
        int depth = (int)goal.transform.position.z;

        Stack<Vector2> path = new Stack<Vector2>();

        while (!((int) cellInfo[row, depth].parentCell.transform.position.x == row && (int)cellInfo[row, depth].parentCell.transform.position.z == depth))
        {
            path.Push(new Vector2(row, depth));
            int tempR = (int) cellInfo[row, depth].parentCell.transform.position.x;
            int tempD = (int)cellInfo[row, depth].parentCell.transform.position.z;
            row = tempR;
            depth = tempD;
        }

        path.Push(new Vector2(row, depth));
        while(path.Count > 0)
        {
            Vector2 p = path.Peek();
            path.Pop();
            mazeGrid[(int) p.x, (int) p.y].ChangeFloorToPath();
        }
    }

    private IEnumerable<MazeCell> GetConnectedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        //Check the cell to the right
        if (x + 1 < mazeWidth)
        {
            var rightCell = mazeGrid[x + 1, z];

            if (rightCell.IsActiveLeftWall() == false && currentCell.IsActiveRightWall() == false)
            {
                yield return rightCell;
            }
        }

        //Check the cell to the left
        if (x - 1 >= 0)
        {
            var leftCell = mazeGrid[x - 1, z];
            if (leftCell.IsActiveRightWall() == false && currentCell.IsActiveLeftWall() == false)
            {
                yield return leftCell;
            }
        }

        //Check the cell to the front
        if (z - 1 >= 0)
        {
            var frontCell = mazeGrid[x, z - 1];
            if (frontCell.IsActiveFrontWall() == false && currentCell.IsActiveBackWall() == false)
            {
                yield return frontCell;
            }
        }

        //Check the cell to the back
        if (z + 1 < mazeDepth)
        {
            var backCell = mazeGrid[x, z + 1];
            if (backCell.IsActiveBackWall() == false && currentCell.IsActiveFrontWall() == false)
            {
                yield return backCell;
            }
        }
    }

    public static double CalculateHValue(int row, int col, MazeCell dest)
    {
        // Return using the distance formula
        return Mathf.Sqrt(Mathf.Pow(row - dest.transform.position.x, 2) + Mathf.Pow(col - dest.transform.position.z, 2));
    }
}