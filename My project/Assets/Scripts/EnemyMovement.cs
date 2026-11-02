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
    public FieldofView foV;
    private Renderer myRenderer;
    private enum State
    {
        Search,
        Suspicion, 
        Chase,
    }
    private State currentState = State.Search;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        navSurface = FindFirstObjectByType<NavMeshSurface>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRenderer = GetComponent<Renderer>();

        // Method A: Direct assignment (Creates a unique material instance clone)
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
                myRenderer.material.color = Color.green;
                agent.SetDestination(searchPoint);
                GetComponent<NavMeshAgent>().speed = 3.5f;
                if (agent.remainingDistance < 1f)
                {
                    // Vector3 point = GenerateRandomPoint();
                    // if (point == searchPoint)
                    // {
                    //     point = GenerateRandomPoint();
                    // }
                    // else
                    // {
                    //     searchPoint = point;
                    // }
                }

                //if (Vector3.Distance(transform.position, target.transform.position) < range)
                //{
                //    currentState = State.Suspicion;

                //    randomPoint = GenerateRandomPoint();

                //    randomPoint.y = transform.position.y;

                //    agent.SetDestination(randomPoint);
                //}
                break;
            case State.Suspicion:
                myRenderer.material.color = Color.yellow;
                agent.SetDestination(pointofInterest);
                
                if (agent.remainingDistance < 1f)
                {
                    currentState = State.Search;
                }
                break;
            case State.Chase:
                myRenderer.material.color = Color.red;
                agent.SetDestination(target.transform.position);
                GetComponent<NavMeshAgent>().speed = 5f;
                if (!foV.canSeePlayer)
                {
                    pointofInterest = target.transform.position;
                    currentState = State.Suspicion;
                }
                
                break;
        }

        if (foV.canSeePlayer)
        {
            currentState = State.Chase;
        }
        Debug.Log(agent.destination);
    }

    public void RebuildNavMesh()
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

    public void InvestigatePoint(Vector3 position)
    {
        //Debug.Log(position);
        currentState = State.Suspicion;
        pointofInterest = position;
    }
}