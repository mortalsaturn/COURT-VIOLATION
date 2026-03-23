using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public DialogueNode[] npcDialogue;   // Each NPC has its own dialogue
    private DialogueManager dialogueManager;

    private bool playerInRange = false;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartNPCDialogue();
        }
    }

    void StartNPCDialogue()
    {
        if (dialogueManager != null)
        {
            dialogueManager.dialogueData.nodes = npcDialogue;
            dialogueManager.StartDialogue();

            // Disable player movement while talking
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var pm = player.GetComponent<PlayerMovement>();
                if (pm) pm.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
