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

        // Method A: Direct assignment (Creates a unique material instance clone)
        RebuildNavMesh();
        target = GameObject.Find("Target");

        mazeGenerator = GameObject.Find("Maze Generator").GetComponent<MazeGenerator>();
        mazeDepth = mazeGenerator._mazeDepth;
        mazeWidth = mazeGenerator._mazeWidth;
        cellSize = mazeGenerator._cellSize;

        agent.updateRotation = false;

        searchPoint = new Vector3((mazeWidth - 1) * cellSize, 0, (mazeDepth - 1) * cellSize);
    }

    void Update()
    {
        target ??= GameObject.Find("Target");
        switch (currentState)
        {
            case State.Search:
                myRenderer.material.color = Color.green;
                agent.SetDestination(searchPoint);
                GetComponent<NavMeshAgent>().speed = 3.5f;
                if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 1f)
                {
                    searchPoint = GenerateRandomPoint();
                }
                break;
            case State.Suspicion:
                myRenderer.material.color = Color.yellow;

                agent.SetDestination(pointofInterest);
                if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 1f)
                {
                    Debug.Log("Sometimes you bug out?");
                    currentState = State.Search;
                }
                // Debug.Log(agent.destination);
                // Debug.Log(pointofInterest);

                // Debug.Log("Path Pending: " + agent.pathPending);
                // Debug.Log("Has Path: " + agent.hasPath);
                // Debug.Log("Path Status: " + agent.pathStatus);
                // Debug.Log("Remaining Distance: " + agent.remainingDistance);
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
                if (agent.remainingDistance < 1f)
                {
                    mySceneManager.QuitApplication();
                }
                break;
        }

        if (foV.canSeePlayer)
        {
            currentState = State.Chase;
            //Time.timeScale = 0f;
        }
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
        GenerateLine();
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
        Debug.Log("we are investgating point.");
        currentState = State.Suspicion;
        pointofInterest = position;
        //agent.SetDestination(pointofInterest);
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