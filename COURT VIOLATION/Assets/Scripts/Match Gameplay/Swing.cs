using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Renderer))]
public class Swing : MonoBehaviour
{
    // ScriptableObjects
    [SerializeField]
    private NewCharacterStats stats;

    // Components
    Animator animator;
    Renderer rend;

    // Other GameObjects' components
    Hitbox hitbox;
    Ball ballInfo;

    // Debugging Properties
    Color baseColor;

    // Stats
    float chargeRate;
    float addedVelocity;

    // Flags
    bool isCharging,
        basicSwinging,
        lobbing;
    public Coroutine chargeRoutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        baseColor = rend.material.color;

        ballInfo = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        hitbox = GetComponentInChildren<Hitbox>();
    }

    void Update()
    {
        if (isCharging)
        {
            rend.material.color = Color.orange;
        }
        else
        {
            rend.material.color = baseColor;
        }
    }

    public void OnSwing(InputAction.CallbackContext context)
    {
        basicSwinging = true;
        ChargeAndSwing(context);
    }

    public void OnLob(InputAction.CallbackContext context)
    {
        lobbing = true;
        ChargeAndSwing(context);
    }

    private void ChargeAndSwing(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction && context.performed)
        {
            if (chargeRoutine == null)
            {
                chargeRoutine = StartCoroutine(Charge());
            }
            isCharging = true;
        }
        if (context.interaction is HoldInteraction && context.canceled)
        {
            if (chargeRoutine != null)
            {
                StopCoroutine(chargeRoutine);
                chargeRoutine = null;
            }
            PerformSwing(Mathf.Min(stats.shotBaseVel + addedVelocity, stats.shotMaxVel));
            addedVelocity = 0;
            isCharging = false;
        }
        if (context.interaction is TapInteraction && context.started)
        {
            PerformSwing(stats.shotBaseVel);
        }
    }

    private IEnumerator Charge()
    {
        while (true)
        {
            addedVelocity += chargeRate * Time.deltaTime;
            yield return null;
        }
    }

    private void PerformSwing(float velocity)
    {
        hitbox.launchVel = velocity;

        if (basicSwinging)
        {
            BasicSwing();
        }
        else if (lobbing)
        {
            Lob();
        }
    }

    private void BasicSwing()
    {
        AnimatorStateChecker();
        if (basicSwinging)
        {
            animator.SetTrigger("Swing");
            animator.SetBool("isBallRight", ballInfo.destination.x >= transform.position.x);
        }
        basicSwinging = false;
    }

    private void Lob()
    {
        AnimatorStateChecker();
        if (lobbing)
        {
            animator.SetTrigger("Lob");
        }
        lobbing = false;
    }

    private void AnimatorStateChecker()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        basicSwinging = !stateInfo.IsName("LeftBasicSwing") && !stateInfo.IsName("RightBasicSwing");
        lobbing = !stateInfo.IsName("Lob");
    }
}
