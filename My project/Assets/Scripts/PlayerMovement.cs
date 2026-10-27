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

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().normalized.x, moveDirection.y, moveAction.ReadValue<Vector2>().normalized.y);
        
        mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit) && placable == 1)
        {
            worldPos = hit.point;
            
            worldPos.x = Mathf.Round(worldPos.x / cellSize) * cellSize;
            worldPos.z = Mathf.Round(worldPos.z / cellSize) * cellSize;
            
            // This is a sihlouette of the wall that will be placed.
            wallSill.transform.position = worldPos;
            if (Mathf.RoundToInt(worldPos.x / cellSize) % 2 != 0 ^ Mathf.RoundToInt(worldPos.z / cellSize) % 2 == 0) // This contained a rounding error :)
            {
                wallSill.transform.rotation =  Quaternion.Euler(0f, 90f, 0f);
            } else
            {
                wallSill.transform.rotation =  Quaternion.Euler(0f, 0f, 0f);
            }

            // if (Mathf.RoundToInt(worldPos.z / cellSize) % 2 == 0) // This contained a rounding error :)
            // {
            //     wallSill.transform.rotation =  Quaternion.Euler(0f, 90f, 0f);
            // } else
            // {
            //     wallSill.transform.rotation =  Quaternion.Euler(0f, 0f, 0f);
            // }
            // Debug.Log(Mathf.RoundToInt(worldPos.x / cellSize) % 2);
            // Debug.Log(Mathf.RoundToInt(worldPos.z / cellSize) % 2);
            

            if (placeAction.WasPressedThisFrame())
            {
                Place(worldPos);   
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
    private void Place(Vector3 worldPos)
    {

        wallInstance = Instantiate(wall, worldPos, Quaternion.identity);
        if (Mathf.RoundToInt(worldPos.x / cellSize) % 2 != 0 ^ Mathf.RoundToInt(worldPos.z / cellSize) % 2 == 0) // This contained a rounding error :)
        {
            wallInstance.transform.rotation =  Quaternion.Euler(0f, 90f, 0f);
        } else
        {
            wallInstance.transform.rotation =  Quaternion.Euler(0f, 0f, 0f);
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
