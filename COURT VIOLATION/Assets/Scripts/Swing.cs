using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    Animator animator;
    Ball ballInfo;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ballInfo = GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>();
    }

    public void OnSwing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformSwing();
        }
    }

    private void PerformSwing()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("LeftBasicSwing") || !stateInfo.IsName("RightBasicSwing"))
        {
            animator.SetTrigger("Swing");
            animator.SetBool("isBallRight", ballInfo.destination.x >= transform.position.x);
        }
    }

    void Update() { }
}
