using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction placeAction;

    private Vector3 moveDirection = Vector3.zero;
    public Rigidbody rb; // set on awake
    public float currentSpeed = 2.0f;

    public float lookSensitivity = 30.0f;
    private Vector3 lookDirection = Vector3.zero;
    private float pitch;
    private float yaw;

    public Camera pCamera;

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        placeAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        placeAction.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        lookAction = inputActions.Player.Look;
        placeAction = inputActions.Player.Place;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().normalized.x, moveDirection.y, moveAction.ReadValue<Vector2>().normalized.y);
        lookDirection = new Vector3(-lookAction.ReadValue<Vector2>().y, lookAction.ReadValue<Vector2>().x, lookDirection.z);
        //Debug.Log(moveDirection);
    }
    private void FixedUpdate()
    {
        Look();
        Move();
    }
    private void Move()
    {
        Vector3 movement = (transform.forward * moveDirection.z) + (transform.right * moveDirection.x);
        rb.MovePosition(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);

        Debug.Log(rb.position + currentSpeed * Time.fixedDeltaTime * movement.normalized);
    }

    private void Look()
    {
        yaw += lookDirection.y * lookSensitivity;
        pitch += lookDirection.x * lookSensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        pCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
