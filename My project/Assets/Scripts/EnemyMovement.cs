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
    private Vector3 randomPoint;
    
    [SerializeField] private float range = 5.0f;
    private enum State
    {
        Search,
        Suspicion, 
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

        agent.updateRotation = true;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Search:
                agent.SetDestination(new Vector3((mazeGenerator._mazeWidth - 1) * mazeGenerator._cellSize, 0, (mazeGenerator._mazeDepth - 1) * mazeGenerator._cellSize));
                if (Vector3.Distance(transform.position, target.transform.position) < range)
                {
                    currentState = State.Suspicion;

                    randomPoint = GenerateRandomPoint();

                    randomPoint.y = transform.position.y;

                    agent.SetDestination(randomPoint);
                }
                break;
            case State.Suspicion: 
                Debug.Log(randomPoint);
                GetComponent<NavMeshAgent>().speed = 1.5f;
                break;
            case State.Chase:
                agent.SetDestination(target.transform.position);
                GetComponent<NavMeshAgent>().speed = 5f;
                break;
        }

        Debug.DrawRay(transform.position, transform.forward * range, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.collider.CompareTag("Target"))
            {
                currentState = State.Chase;
            }
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

    public Vector3 GenerateRandomPoint()
    {
        NavMeshPath path = new NavMeshPath();
        randomPoint = target.transform.position + (Random.insideUnitSphere * 3f);

        if (NavMesh.CalculatePath(transform.position, randomPoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return randomPoint;
        } 
        return GenerateRandomPoint();
    }
}