using UnityEngine;

public class MainCameraCentre : MonoBehaviour
{
    public MazeGenerator mazeGenerator;
    private float centreX;
    private float centreZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float cellSize = mazeGenerator._cellSize;
        float mazeDepth = mazeGenerator._mazeDepth;
        float mazeWidth = mazeGenerator._mazeWidth;

        centreX = mazeDepth * cellSize / 2f;
        centreZ = mazeWidth * cellSize / 2f;

        transform.position = new Vector3(centreX, 5f, centreZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
