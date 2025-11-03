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
    Animator animator;
    Color baseColor;
    Ball ballInfo;

    [SerializeField]
    float chargeRate,
        addedVelocity;
    bool isCharging;
    public Coroutine chargeRoutine;
    Hitbox[] hitboxes;
    Renderer rend;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<Renderer>();
    }

    void Start()
    {
        baseColor = rend.material.color;

        ballInfo = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
        hitboxes = GetComponentsInChildren<Hitbox>();
    }

    public void OnSwing(InputAction.CallbackContext context)
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
            PerformSwing(Mathf.Min(hitboxes[0].baseVel + addedVelocity, hitboxes[0].maxVel));
            addedVelocity = 0;
            isCharging = false;
        }
        if (context.interaction is TapInteraction && context.started)
        {
            PerformSwing(hitboxes[0].baseVel);
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
        Debug.Log("HB Vel: " + velocity);
        foreach (var hb in hitboxes)
        {
            hb.launchVel = velocity;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("LeftBasicSwing") || !stateInfo.IsName("RightBasicSwing"))
        {
            animator.SetTrigger("Swing");
            animator.SetBool("isBallRight", ballInfo.destination.x >= transform.position.x);
        }
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
}
