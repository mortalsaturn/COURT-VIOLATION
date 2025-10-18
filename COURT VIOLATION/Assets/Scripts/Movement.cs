using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed;
    Rigidbody rigidbody;
    private Vector3 _moveDirection;

    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction quickStepAction;

    void OnEnable()
    {
        inputActions.FindActionMap("Gameplay").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Gameplay").Disable();
    }

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        quickStepAction = InputSystem.actions.FindAction("Quick Step");

        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveDirection = moveAction.ReadValue<Vector2>();
        if (true) { }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        transform.position += new Vector3(
            _moveDirection.x * speed * Time.deltaTime,
            0f,
            _moveDirection.y * speed * Time.deltaTime
        );
    }
}
