/////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private MazeCell _mazeCellPrefab;
    public MySceneManager  mySceneManager;
    public MainCameraCentre mainCameraCentre;
    public int _mazeWidth;
    public int _mazeDepth;
    public float _cellSize;
    public GameObject enemy;
    public int lessWalls;
    private MazeCell[,] _mazeGrid;
    void Start()
    {
        mySceneManager = MySceneManager.Instance;

        _mazeWidth = mySceneManager.mazeWidth;
        _mazeDepth = mySceneManager.mazeDepth;
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            // if x == 0 || x == _mazeWidth - 1, then it's the edge
            // if z == 0 || z == _MazeDepth - 1, then it's also the edge
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector3 position = new Vector3(x * _cellSize, 0, z * _cellSize);
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, position, Quaternion.identity);
            }
        }
        GenerateMaze(null, _mazeGrid[0, 0]);
        
        target.transform.position = new Vector3((_mazeWidth - 1)/2 * _cellSize, 1, 
                                                (_mazeDepth - 1)/2 * _cellSize);
        //Debug.Log(_mazeDepth == mySceneManager.mazeDepth);
        Instantiate(enemy, new Vector3(0,1,0), Quaternion.identity);
        mainCameraCentre.RepositionCamera();

        lessWalls = Mathf.RoundToInt(_mazeDepth * _mazeWidth / 4);
        RemoveMoreWalls();
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

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x / _cellSize);
        int z = Mathf.RoundToInt(currentCell.transform.position.z / _cellSize);

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

        if (z + 1 < _mazeDepth)
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

        // If the previous cell is to the left of the current cell
        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        // If the previous cell is to the right of the current cell
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        // If the previous cell is below the current cell
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        // If the previous cell is above the current cell
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            Debug.Log("Do we ever go here, I don't know.");
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }


    }

    private void RemoveMoreWalls()
    {
        for (int i = 0; i < lessWalls; i++)
        {
            int randomX = Random.Range(1, _mazeWidth - 1);
            int randomZ = Random.Range(1, _mazeDepth - 1);

            MazeCell randomCell = _mazeGrid[randomX, randomZ];
            //int randomNum = Random.Range(0, 4);
            // switch (randomNum)
            // {
            //     case 0:
            //         randomCell.ClearFrontWall();
            //         _mazeGrid[randomX, randomZ + 1].ClearBackWall();
            //         break;
            //     case 1:
            //         randomCell.ClearLeftWall();
            //         _mazeGrid[randomX - 1, randomZ].ClearRightWall();
            //         break;
            //     case 2:
            //         randomCell.ClearBackWall();
            //         _mazeGrid[randomX, randomZ - 1].ClearFrontWall();
            //         break;
            //     case 3:
            //         randomCell.ClearRightWall();
            //         _mazeGrid[randomX + 1, randomZ].ClearLeftWall();
            //         break;
            // }
            randomCell.ClearFrontWall();
            randomCell.ClearLeftWall();
            randomCell.ClearBackWall();
            randomCell.ClearRightWall();
            
            // Debug.Log(randomNum);
            // Debug.Log(new Vector2(randomX, randomZ));

        }
    }

}