using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  
    }

    void Update()
    {
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        input = new Vector3(x, 0f, z).normalized;
    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }
}
