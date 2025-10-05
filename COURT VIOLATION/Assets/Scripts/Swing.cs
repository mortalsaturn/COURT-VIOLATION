using UnityEngine;

public class Swing : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");
            animator.SetBool("basicSwing", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("basicSwing", false);
        }
    }
}
