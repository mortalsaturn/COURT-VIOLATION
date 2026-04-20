using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
   public DialogueManager DialogueManager;
   public CameraManager cameraManager;
    public GameObject promptUI;
    public float triggerDistance = 2.5f;
    private float timer = 0f;
    public float delayBeforePrompt = 3f;
    private bool canShowPrompt = false;

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
            timer += Time.deltaTime;

            // Wait 3 seconds before showing prompt
            if (timer >= delayBeforePrompt)
            {
                canShowPrompt = true;
            }

            if (canShowPrompt)
            {
                promptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.T))
                {
                    isInDialogue = true;
                    StartDialogue();
                }
            }
        }
        else
        {
            // Reset everything when player leaves
            promptUI.SetActive(false);
            timer = 0f;
            canShowPrompt = false;
        }   
    }

    void StartDialogue()
    {
       promptUI.SetActive(false);

        // Lock player movement
        PlayerMovement controller = player.GetComponent<PlayerMovement>();
        if (controller != null)
            controller.enabled = false;

        //switch to NPC camera
        if (cameraManager != null)
            cameraManager.SetCamera(CameraManager.CamState.NPC);

        // start dialogue
        DialogueManager.StartDialogue();
    }

   
    public void EndDialogue()
    {
        isInDialogue = false;
    }
}
