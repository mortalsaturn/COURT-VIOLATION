using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject promptUI;
    public float triggerDistance = 2.5f;

    private Transform player;

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
            promptUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartDialogue();
            }
        }
        else
        {
            promptUI.SetActive(false);
        }
    }

    void StartDialogue()
    {
        // Turn off prompt
        promptUI.SetActive(false);

        // Lock player movement
        PlayerMovement controller = player.GetComponent<PlayerMovement>();
        if (controller != null)
            controller.enabled = false;

        // SWITCH CAMERA IMMEDIATELY 
        if (dialogueManager != null && dialogueManager.cameraManager != null)
            dialogueManager.cameraManager.SetCamera(CameraManager.CamState.PLAYER);

        // Start the dialogue system
        dialogueManager.StartDialogue();
    }
}
