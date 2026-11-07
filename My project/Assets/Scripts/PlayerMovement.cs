using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.AI.Navigation;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction placeAction;
    private InputAction removeAction;

    [SerializeField] private NavMeshSurface navSurface;
    private Vector3 moveDirection = Vector3.zero;
    public Rigidbody rb; // set on awake
    public float currentSpeed = 2.0f;
    private GameObject wallInstance;
    public GameObject wall;
    public GameObject wallSill;
    private int placable = 1;
    private Vector2 mousePos;
    private Vector3 worldPos;

    public MazeGenerator mazeGenerator;
    public EnemyMovement enemyMovement;

    private float cellSize;

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        placeAction.Enable();
        removeAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        placeAction.Disable();
        removeAction.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        lookAction = inputActions.Player.Look;
        placeAction = inputActions.Player.Place;
        removeAction = inputActions.Player.Attack;
        //Cursor.lockState = CursorLockMode.Locked;

        cellSize = mazeGenerator._cellSize;
    }

    public void GetEnemy()
    {
        GameObject enemy = GameObject.Find("Enemy(Clone)");
        enemyMovement = enemy.GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationAngle = new Vector3(0f, 0f, 0f);
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().normalized.x, moveDirection.y, moveAction.ReadValue<Vector2>().normalized.y);

        mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit) && placable == 1)
        {
            worldPos = hit.point;

            // Find the centre of the nearest cell
            float halfCell = cellSize / 2f;

            float cellX = Mathf.Round(worldPos.x / cellSize) * cellSize;
            float cellZ = Mathf.Round(worldPos.z / cellSize) * cellSize;

            // Calculate distance to each of the four edges
            float left = Mathf.Abs(worldPos.x - (cellX - halfCell));
            float right = Mathf.Abs(worldPos.x - (cellX + halfCell));
            float bottom = Mathf.Abs(worldPos.z - (cellZ - halfCell));
            float top = Mathf.Abs(worldPos.z - (cellZ + halfCell));

            // Determine which orientation is closest
            bool verticalEdge = Mathf.Min(left, right) < Mathf.Min(top, bottom);

            if (verticalEdge)
            {
                rotationAngle.y = 0f;
                // Left or right edge
                worldPos.x = left < right ? cellX - halfCell : cellX + halfCell;
                worldPos.z = cellZ;

                wallSill.transform.rotation = Quaternion.Euler(rotationAngle);
            }
            else
            {
                rotationAngle.y = 90f;
                // Top or bottom edge
                worldPos.x = cellX;
                worldPos.z = bottom < top ? cellZ - halfCell : cellZ + halfCell;

                wallSill.transform.rotation = Quaternion.Euler(rotationAngle);
            }

            // Position the wall silhouette
            wallSill.transform.position = new Vector3(worldPos.x, 0f, worldPos.z);


            if (placeAction.WasPressedThisFrame())
            {
                Place(worldPos, rotationAngle, verticalEdge);
            }


        }

        if (removeAction.WasPressedThisFrame() && placable == 0)
        {
            Remove();
        }


    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        Vector3 movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);
        rb.MovePosition(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);

        //Debug.Log(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);
    }
    private void Place(Vector3 worldPos, Vector3 angle, bool vertical)
    {

        wallInstance = Instantiate(wall, new Vector3(worldPos.x, 0f, worldPos.z), Quaternion.identity);
        if (vertical)
        {
            wallInstance.transform.rotation = Quaternion.Euler(angle);
        }
        else
        {
            wallInstance.transform.rotation = Quaternion.Euler(angle);
        }
        placable = 0;
        wallSill.SetActive(false);
        //navSurface.BuildNavMesh();
        enemyMovement.InvestigatePoint(worldPos);
    }

    private void Remove()
    {
        placable = 1;
        wallSill.SetActive(true);

        Destroy(wallInstance);

    }

}
