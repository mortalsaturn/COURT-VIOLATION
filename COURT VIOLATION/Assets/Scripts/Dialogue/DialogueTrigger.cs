using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
   public DialogueManager DialogueManager;
   public CameraManager cameraManager;
    public GameObject promptUI;
    public float triggerDistance = 2.5f;

    private Transform player;
    private bool isInDialogue = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        promptUI.SetActive(false);
    }

    void Update()
    {
        if (isInDialogue) 
        {
            
            promptUI.SetActive(false);
            return;
        }

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= triggerDistance)
        {
            promptUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.T))
            {
                isInDialogue = true;    
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
        promptUI.SetActive(false);

        // Lock player movement
        PlayerMovement controller = player.GetComponent<PlayerMovement>();
        if (controller != null)
            controller.enabled = false;

        // Switch camera
        if (cameraManager != null && cameraManager != null)
            cameraManager.SetCamera(CameraManager.CamState.PLAYER);

        // Start the dialogue
        DialogueManager.StartDialogue();
    }

   
    public void EndDialogue()
    {
        isInDialogue = false;
    }
}
