using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private GameObject target;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private NavMeshSurface navSurface;
    public MazeGenerator mazeGenerator;
    
    [SerializeField] private float range = 10.0f;
    private enum State
    {
        Search,
        Chase,
    }
    private State currentState = State.Search;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        navSurface = FindFirstObjectByType<NavMeshSurface>();
        RebuildNavMesh();
        target = GameObject.Find("Target");

        mazeGenerator = GameObject.Find("Maze Generator").GetComponent<MazeGenerator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Search:
                agent.SetDestination(new Vector3((mazeGenerator._mazeWidth - 1) * mazeGenerator._cellSize, 0, (mazeGenerator._mazeDepth - 1) * mazeGenerator._cellSize));
                break;
            case State.Chase:
                break;
        }
        //Debug.Log(target.transform.position);
        // if (NavMesh.SamplePosition(target.transform.position, out NavMeshHit hit, range, NavMesh.AllAreas))
        // {
        //     //Debug.Log("Found NavMesh at: " + hit.position);
        //     Debug.DrawRay(targetTransform.position, Vector3.up, Color.blue, 1.0f);
        // }
        // else
        // {
        //     //Debug.Log("No NavMesh found nearby.");
        // }
        // if (target != null)
        // {
        //     agent.SetDestination(targetTransform.position);
        // }
        //  if (agent.hasPath)
        // {
        //     // Vector3[] array containing all upcoming path corners (waypoints)
        //     Vector3[] pathWaypoints = agent.path.corners;

        //     // Example: Loop through and print them or draw lines
        //     for (int i = 0; i < pathWaypoints.Length; i++)
        //     {
        //         Debug.Log($"Waypoint {i}: {pathWaypoints[i]}");
        //     }
        // }
        //Debug.Log((mazeGenerator._mazeDepth - 1) * mazeGenerator._cellSize);
    }

    void RebuildNavMesh()
    {
        navSurface.BuildNavMesh();
    }
}