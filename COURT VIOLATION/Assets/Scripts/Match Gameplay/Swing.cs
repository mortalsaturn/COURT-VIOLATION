using System;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;

public enum ServingState
{
    Serving,
    Playing,
}

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
    public Ball ball;

    // Debugging Properties
    Color baseColor;

    // Stats
    float chargeRate;
    float addedVelocity;

    // Flags
    bool isCharging,
        hasCharged,
        serving,
        basicSwinging,
        lobbing,
        dropShotting;
    public Coroutine chargeRoutine;
    public ServingState servingState;
    public Transform ballServeSpawn;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        baseColor = rend.material.color;
        if (servingState == ServingState.Serving)
        {
            animator.SetBool("isServing", true);
        }

        ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
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

    public void OnDropShot(InputAction.CallbackContext context)
    {
        dropShotting = true;
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
            hasCharged = true;
            if (chargeRoutine != null)
            {
                StopCoroutine(chargeRoutine);
                chargeRoutine = null;
            }
            PerformSwing();
            addedVelocity = 0;
            isCharging = false;
        }
        if (context.interaction is TapInteraction && context.started)
        {
            hasCharged = false;
            PerformSwing();
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

    private IEnumerator ApplyServeForce()
    {
        yield return null; // Wait one frame for physics to update
        ball.rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
    }

    private void PerformSwing()
    {
        if (servingState == ServingState.Serving)
        {
            if (!ball.gameObject.activeInHierarchy)
            {
                ball.gameObject.SetActive(true);
            }
            ball.transform.position = ballServeSpawn.position;
            StartCoroutine(ApplyServeForce());
        }
        else
        {
            if (basicSwinging)
            {
                hitbox.shotType = ShotType.BasicShot;

                hitbox.launchVel = hasCharged
                    ? Mathf.Min(stats.basicShotBaseVel + addedVelocity, stats.basicShotMaxVel)
                    : stats.basicShotBaseVel;

                BasicSwing();
            }
            else if (lobbing)
            {
                hitbox.shotType = ShotType.Lob;

                hitbox.launchVel = hasCharged
                    ? Mathf.Min(stats.lobBaseVel + addedVelocity, stats.lobMaxVel)
                    : stats.lobBaseVel;

                Lob();
            }
            else if (dropShotting)
            {
                hitbox.shotType = ShotType.DropShot;

                hitbox.launchVel = hasCharged
                    ? Mathf.Min(stats.dropShotBaseVel + addedVelocity, stats.dropShotMaxVel)
                    : stats.dropShotBaseVel;

                DropShot();
            }
        }
    }

    private void BasicSwing()
    {
        AnimatorStateChecker();
        if (!basicSwinging)
        {
            animator.SetTrigger("Swing");
            animator.SetBool("isBallRight", ball.destination.x >= transform.position.x);
        }
    }

    private void Lob()
    {
        AnimatorStateChecker();
        if (!lobbing)
        {
            animator.SetTrigger("Lob");
        }
    }

    private void DropShot()
    {
        AnimatorStateChecker();
        if (!dropShotting)
        {
            animator.SetTrigger("DropShot");
        }
    }

    private void AnimatorStateChecker()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        serving = stateInfo.IsName("ServeStance");
        basicSwinging = stateInfo.IsName("LeftBasicSwing") || stateInfo.IsName("RightBasicSwing");
        lobbing = stateInfo.IsName("Lob");
        dropShotting = stateInfo.IsName("DropShot");
    }
}
