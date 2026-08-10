using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;

    private Vector3 moveDirection = Vector3.zero;
    public Rigidbody rb; // set on awake
    public float currentSpeed = 2.0f;

    private void OnEnable()
    {
        moveAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().normalized.x, moveDirection.y, moveAction.ReadValue<Vector2>().normalized.y);
        //Debug.Log(moveDirection);
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void Move()
    {
        Vector3 movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);
        rb.MovePosition(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);

        Debug.Log(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);
    }
}
