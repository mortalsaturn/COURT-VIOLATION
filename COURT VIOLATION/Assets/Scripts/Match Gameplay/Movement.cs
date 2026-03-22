using System.Collections;
using JetBrains.Annotations;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
public class Movement : MonoBehaviour
{
    [SerializeField]
    NewCharacterStats stats;
    float baseSpeed;
    float sprintModifier;
    float quickStepDistance,
        quickStepDuration;
    float jumpForce;
    Renderer rend;
    Color baseColor;
    private Vector3 moveDirection;
    private bool isSprinting;
    private bool quickStepRequested,
        isQuickStepping;

    GameObject ball;
    Rigidbody rb;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();

        baseSpeed = stats.baseSpeed;
        sprintModifier = stats.sprintModifier;
        quickStepDistance = stats.quickStepDistance;
        quickStepDuration = stats.quickStepDuration;
        jumpForce = stats.jumpForce;
    }

    void Start()
    {
        baseColor = rend.material.color;
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }

    public void OnQuickStep(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            quickStepRequested = true;
            QuickStep();
        }
    }

    void Update()
    {
        if (isSprinting)
        {
            rend.material.color = Color.green;
        }
        else if (isQuickStepping)
        {
            rend.material.color = Color.yellow;
        }
        else
        {
            rend.material.color = baseColor;
        }
    }

    void FixedUpdate()
    {
        Move();
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.down * 1.5f);
        }
    }

    private void Move()
    {
        if (!isSprinting)
        {
            transform.position += new Vector3(
                moveDirection.x * baseSpeed * Time.deltaTime,
                0f,
                moveDirection.y * baseSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.position += new Vector3(
                moveDirection.x * baseSpeed * sprintModifier * Time.deltaTime,
                0f,
                moveDirection.y * baseSpeed * sprintModifier * Time.deltaTime
            );
        }
    }

    private void QuickStep()
    {
        if (quickStepRequested && !isQuickStepping)
        {
            StartCoroutine(QuickStepRoutine());
            quickStepRequested = false;
        }
    }

    private IEnumerator QuickStepRoutine()
    {
        isQuickStepping = true;

        Vector3 startPos = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );

        Vector3 ballPos = new Vector3(
            ball.transform.position.x,
            transform.position.y,
            ball.transform.position.z
        );
        Vector3 directionToBall = (ballPos - startPos).normalized;

        Vector3 targetPos = startPos + directionToBall * quickStepDistance;

        float elapsed = 0f;

        while (elapsed < quickStepDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / quickStepDuration);
            float distanceToBall = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                ballPos
            );

            float stopDistance = 2;
            if (distanceToBall <= stopDistance)
            {
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        isQuickStepping = false;
    }
}
