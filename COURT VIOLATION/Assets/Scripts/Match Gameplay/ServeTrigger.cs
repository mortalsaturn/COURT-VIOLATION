using Unity.VisualScripting;
using UnityEngine;

public class ServeTrigger : MonoBehaviour
{
    private Swing swing;

    private void Awake()
    {
        swing = GetComponentInParent<Swing>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        swing.animator.SetBool("isServeThrowing", false);
        swing.isBallAirborne = false;
        other.gameObject.SetActive(false);
    }
}
