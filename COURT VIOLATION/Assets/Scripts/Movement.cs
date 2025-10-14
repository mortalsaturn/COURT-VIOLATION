using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed;
    Rigidbody rigidbody;
    private Vector3 _moveDirection;
    public InputActionReference move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveDirection = move.action.ReadValue<Vector3>();
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(
            _moveDirection.x * speed * Time.deltaTime,
            0f,
            _moveDirection.z * speed * Time.deltaTime
        );
    }
}
