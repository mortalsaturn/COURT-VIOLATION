using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float baseSpeed;
    public float sprintModifier;
    public float quickStepDistance,
        quickStepDuration;

    Rigidbody rigidbody;
    Renderer renderer;
    Color baseColor;
    private Vector3 moveDirection;
    private bool isSprinting;
    private bool quickStepRequested,
        isQuickStepping;

    public GameObject ball;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        baseColor = renderer.material.color;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
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
            renderer.material.color = Color.green;
        }
        else if (isQuickStepping)
        {
            renderer.material.color = Color.yellow;
        }
        else
        {
            renderer.material.color = baseColor;
        }
    }

    void FixedUpdate()
    {
        Move();
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
