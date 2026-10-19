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
    private NavMeshHit hit;
    private float mazeDepth;
    private float mazeWidth;
    private float cellSize;
    private Vector3 pointofInterest;
    private Vector3 searchPoint;
    
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
        mazeDepth = mazeGenerator._mazeDepth;
        mazeWidth = mazeGenerator._mazeWidth;
        cellSize = mazeGenerator._cellSize;

        agent.updateRotation = true;

        searchPoint = new Vector3((mazeWidth - 1) * cellSize, 0, (mazeDepth - 1) * cellSize);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Search:
                agent.SetDestination(searchPoint);
                GetComponent<NavMeshAgent>().speed = 3.5f;
                if (agent.remainingDistance < 1f)
                {
                    Vector3 point = GenerateRandomPoint();
                    if (point == searchPoint)
                    {
                        point = GenerateRandomPoint();
                    }
                    else
                    {
                        searchPoint = point;
                    }
                }

                if (!NavMesh.Raycast(transform.position, target.transform.position, out hit, NavMesh.AllAreas))
                {
                    currentState = State.Chase;
                }

                // if (Vector3.Distance(transform.position, target.transform.position) < range)
                // {
                //     currentState = State.Suspicion;

                //     randomPoint = GenerateRandomPoint();

                //     randomPoint.y = transform.position.y;

                //     agent.SetDestination(randomPoint);
                // }
                break;
            case State.Suspicion:
                agent.SetDestination(pointofInterest);
                
                if (!NavMesh.Raycast(transform.position, target.transform.position, out hit, NavMesh.AllAreas))
                {
                    currentState = State.Chase;
                }
                if (agent.remainingDistance < 1f)
                {
                    if (NavMesh.Raycast(transform.position, target.transform.position, out hit, NavMesh.AllAreas))
                    {
                        currentState = State.Search;
                    }
                }
                break;
            case State.Chase:
                agent.SetDestination(target.transform.position);
                GetComponent<NavMeshAgent>().speed = 5f;
                if (NavMesh.Raycast(transform.position, target.transform.position, out hit, NavMesh.AllAreas))
                {
                    pointofInterest = target.transform.position;
                    currentState = State.Suspicion;
                }
                break;
        }
        bool blocked = false;
        blocked = NavMesh.Raycast(transform.position, target.transform.position, out hit, NavMesh.AllAreas);
        Debug.DrawLine(transform.position, target.transform.position, blocked ? Color.red : Color.green);
        Debug.Log(currentState);
    }

    void RebuildNavMesh()
    {
        navSurface.BuildNavMesh();
    }

    public Vector3 GenerateRandomPoint()
    {
        NavMeshPath path = new NavMeshPath();
        float randomX = Random.Range(0f, (mazeDepth - 1) * cellSize);
        float randomZ = Random.Range(0f, (mazeWidth - 1) * cellSize);
        Vector3 randomPoint = new Vector3(randomX, 0, randomZ);

        if (NavMesh.CalculatePath(transform.position, randomPoint, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return randomPoint;
        } 
        return GenerateRandomPoint();
    }
}