using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeGeneratorInstant : MonoBehaviour
{
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

    public TextMeshProUGUI blueScoreText;
    private int blueScore = 0;
    public TextMeshProUGUI greenScoreText;
    private int greenScore = 0;
    public TextMeshProUGUI yellowScoreText;
    private int yellowScore = 0;
    public static bool first = true;

    public GameObject music;
    public AudioSource goalSFX;

    // Start is called before the first frame update
    void Awake()
    {
        if(first)
        {
            PlayerPrefs.DeleteAll();
            first = false;
            music.SetActive(true);
        }
        else
        {
            blueScore = PlayerPrefs.GetInt("Blue Score", 0);
            blueScoreText.text = blueScore.ToString();
            greenScore = PlayerPrefs.GetInt("Green Score", 0);
            greenScoreText.text = greenScore.ToString();
            yellowScore = PlayerPrefs.GetInt("Yellow Score", 0);
            yellowScoreText.text = yellowScore.ToString();
        }
    }

    void Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];

        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                //If the cell is in the middle of the maze, generate an instance of the maze goal prefab
                //This condition is hardcoded for a maze of equal dimensions and there are more than 1 cells in the maze
                if((i == ((mazeWidth - 1) / 2) && j == ((mazeDepth - 1) / 2)) || //Top Left
                    (i == ((mazeWidth - 1) / 2) + 1 && j == ((mazeDepth - 1) / 2)) || //Top Right
                    (i == ((mazeWidth - 1) / 2) && j == ((mazeDepth - 1) / 2) + 1) || //Bottom Left
                    (i == ((mazeWidth - 1) / 2) + 1 && j == ((mazeDepth - 1) / 2) + 1)) // Bottom Right
                {
                    mazeGrid[i, j] = Instantiate(mazeGoalPrefab, new Vector3(i, 0, j), Quaternion.identity, gameObject.transform);
                }
                else //Otherwise, generate a regular maze cell
                {
                    mazeGrid[i, j] = Instantiate(mazeCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
                    //Set Boundary Left 
                    if(i == 0)
                    {
                        mazeGrid[i, j].BoundaryLeft();
                    }
                    //Set Boundary Right
                    if(i == mazeWidth - 1)
                    {
                        mazeGrid[i, j].BoundaryRight();
                    }
                    //Set Boundary Front
                    if(j == mazeDepth - 1)
                    {
                        mazeGrid[i, j].BoundaryFront();
                    }
                    //Set Boundary Back
                    if(j == 0)
                    {
                        mazeGrid[i, j].BoundaryBack();
                    }
                }
                
            }
        }
        StartAtRandomEdgeCell(true);
    }
    void Update()
    {

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

                GenerateMaze(null, mazeGrid[xStartPoint, zStartPoint]);
                break;
            case 2:
                edgeChooser = Mathf.RoundToInt(Random.Range(1.0f, 2.0f));
                zStartPoint = edgeChooser % 2 == 0 ? 0 : mazeDepth - 1;
                xStartPoint = Mathf.RoundToInt(Random.Range(0.0f, (mazeWidth - 1)));

                GenerateMaze(null, mazeGrid[xStartPoint, zStartPoint]);
                break;
            default:
                xStartPoint = 0;
                zStartPoint = 0;
                break;
        }

        if(isFirstRound)
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
    private void SpawnPlayers(int xStart,  int zStart)
    {
        int edge = CheckEdge(xStart, zStart);
        int distance = GetDistance(xStart, zStart, edge);

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if(i == 0)
            {
                Instantiate(playerPrefabList[i], new Vector3((float)xStart, 0.35f, (float)zStart), Quaternion.identity, playersParent.transform);
                if(edge == 3)
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
        if((x >= 0 && x <= mazeWidth - 2) && z == 0)
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
        switch(winner)
        {
            case 1:
                blueScore++;
                blueScoreText.text = blueScore.ToString();
                PlayerPrefs.SetInt("Blue Score", blueScore);
                break;
            case 2:
                greenScore++;
                greenScoreText.text = greenScore.ToString();
                PlayerPrefs.SetInt("Green Score", greenScore);
                break;
            case 3:
                yellowScore++;
                yellowScoreText.text = yellowScore.ToString();
                PlayerPrefs.SetInt("Yellow Score", yellowScore);
                break;
            default:
                break;
        }
        //Cahnge to 5
        if(blueScore == 5 || greenScore == 5 || yellowScore == 5) {
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

    /// <summary>
    /// Moves all players outside the camera
    /// </summary>
    private void MovePlayersToWaitPoint()
    {
        for(int i = 0; i < numberOfPlayers; i++)
        {
            playersParent.transform.GetChild(i).gameObject.transform.position = new Vector3(0.0f, 0.0f, -100.0f);
        }
    }
}