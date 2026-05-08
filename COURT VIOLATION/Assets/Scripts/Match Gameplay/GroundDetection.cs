using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private Movement movement;
    private Rigidbody rb;
    private bool isGrounded = false;

    private void Awake()
    {
        movement = GetComponentInParent<Movement>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor") && !isGrounded)
        {
            isGrounded = true;
            movement.animator.SetBool("isAirborne", false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor") && !isGrounded)
        {
            isGrounded = true;
            movement.animator.SetBool("isAirborne", false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor") && isGrounded)
        {
            isGrounded = false;
            movement.animator.SetBool("isAirborne", true);
        }
    }
}
