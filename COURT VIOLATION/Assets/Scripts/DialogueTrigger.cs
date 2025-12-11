using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject promptUI; // �Press T to talk�
    public float triggerDistance = 2.5f;

    private Transform player;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        promptUI.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= triggerDistance)
        {
            playerInRange = true;
            promptUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartDialogue();
            }
        }
        else
        {
            playerInRange = false;
            promptUI.SetActive(false);
        }
    }

    void StartDialogue()
    {
        promptUI.SetActive(false);

        // Lock player movement
        PlayerMovement controller = player.GetComponent<PlayerMovement>();
        if (controller != null)
            controller.enabled = false;

        // Activate dialogue system
        dialogueManager.StartDialogue();
    }
}
