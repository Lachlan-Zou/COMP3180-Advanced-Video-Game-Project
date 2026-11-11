using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    private GameObject target;
    private NavMeshAgent agent;
    private NavMeshSurface navSurface;
    [SerializeField] private float turnSpeed;
    public MazeGenerator mazeGenerator;
    private float mazeDepth;
    private float mazeWidth;
    private float cellSize;
    private Vector3 pointofInterest;
    private Vector3 searchPoint;
    public FieldofView foV;
    private Renderer myRenderer;
    public GameObject theSceneManager;
    public MySceneManager mySceneManager;
    private LineRenderer lineRenderer;
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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mySceneManager = theSceneManager.GetComponent<MySceneManager>();
        myRenderer = GetComponent<Renderer>();

        target = GameObject.Find("Target");

        mazeGenerator = GameObject.Find("Maze Generator").GetComponent<MazeGenerator>();
        mazeDepth = mazeGenerator._mazeDepth;
        mazeWidth = mazeGenerator._mazeWidth;
        cellSize = mazeGenerator._cellSize;

        agent.updateRotation = false;

        searchPoint = new Vector3((mazeWidth - 1) * cellSize, 0, (mazeDepth - 1) * cellSize);
        EnterSearch();
    }

    void Update()
    {
        target ??= GameObject.Find("Target");
        switch (currentState)
        {
            case State.Search:
                if (!agent.pathPending && !agent.hasPath) // If the search point is invalid
                {
                    EnterSearch();
                }

                if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 1f)
                {
                    EnterSearch();
                }

                break;
            case State.Suspicion:
                if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 1f)
                {
                    EnterSearch();
                }
                break;
            case State.Chase:
                if (!foV.canSeePlayer)
                {
                    pointofInterest = target.transform.position;
                    InvestigatePoint(pointofInterest);
                }
                if (agent.remainingDistance < 1f)
                {
                    mySceneManager.QuitApplication();
                }
                break;
        }

        if (foV.canSeePlayer)
        {
            EnterChase();
        }
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
        GenerateLine();

        //Debug.Log(agent.remainingDistance);
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

    public void EnterSearch()
    {
        searchPoint = GenerateRandomPoint();
        currentState = State.Search;
        myRenderer.material.color = Color.green;
        agent.SetDestination(searchPoint);
        GetComponent<NavMeshAgent>().speed = 3.5f;
    }
    public void InvestigatePoint(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        bool success = agent.CalculatePath(position, path); // is the path valid

        if (success && path.status == NavMeshPathStatus.PathComplete)
        {
            currentState = State.Suspicion;
            myRenderer.material.color = Color.yellow;

            pointofInterest = position;

            // You better not bug out on me anymore.
            agent.ResetPath();

            bool destinationSet = agent.SetDestination(pointofInterest);
        }
    }
    public void EnterChase()
    {
        currentState = State.Chase;
        myRenderer.material.color = Color.red;
        agent.SetDestination(target.transform.position);
        GetComponent<NavMeshAgent>().speed = 5f;
    }
    public void GenerateLine()
    {
        if (agent.hasPath)
        {
            // Get the corners of the calculated path
            Vector3[] corners = agent.path.corners;
            lineRenderer.positionCount = corners.Length;

            for (int i = 0; i < corners.Length; i++)
            {
                // Slightly lift the line off the floor to prevent z-fighting
                lineRenderer.SetPosition(i, corners[i] + Vector3.up * 0.1f);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}