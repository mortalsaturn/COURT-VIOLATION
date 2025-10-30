using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swing : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start() { }

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

        if (!stateInfo.IsName("BasicSwing"))
        {
            animator.SetTrigger("Swing");
        }
    }
}
