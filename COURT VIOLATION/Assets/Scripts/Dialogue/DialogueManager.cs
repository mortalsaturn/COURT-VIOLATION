using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Core")]
    public DialogueData dialogueData;

    [Header("Systems")]
    public TextBubbleUI textBubbleUI;
    public PortraitUI portraitUI;
    public UIReferences ui;
    public CameraManager cameraManager;
    public SceneLoader sceneLoader;

    [Header("Scene")]
    public string nextSceneName;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private int currentNodeIndex;
    private bool isTyping;
    private Coroutine typingRoutine;

    // ✅ NEW: Dialogue phases
    enum DialoguePhase { NONE, NPC, PLAYER }
    private DialoguePhase currentPhase;
    private DialogueNode currentNode;

    // =========================
    // START
    // =========================
    public void StartDialogue()
    {
        currentNodeIndex = 0;
        DisplayNode();
    }

    // =========================
    // DISPLAY NODE
    // =========================
    void DisplayNode()
    {
        if (!dialogueData.IsValidNode(currentNodeIndex))
        {
            StartCoroutine(EndDialogue());
            return;
        }

        currentNode = dialogueData.GetNode(currentNodeIndex);

        StopTyping();
        ui.HideChoices();
        textBubbleUI.HideAll();

        // Decide who talks first
        if (!string.IsNullOrEmpty(currentNode.npcText))
        {
            currentPhase = DialoguePhase.NPC;
            typingRoutine = StartCoroutine(TypeNPC());
        }
        else if (!string.IsNullOrEmpty(currentNode.playerText))
        {
            currentPhase = DialoguePhase.PLAYER;
            typingRoutine = StartCoroutine(TypePlayer());
        }
    }

    // =========================
    // NPC TYPE
    // =========================
    IEnumerator TypeNPC()
    {
        isTyping = true;

        cameraManager?.SetCamera(CameraManager.CamState.NPC);

        textBubbleUI.ShowNPCBubble(currentNode.npcID);
        portraitUI.ShowNPC(currentNode.npcID, currentNode.npcPortrait);

        yield return TypeText(currentNode.npcText, ui.npcTextBox);

        isTyping = false;
    }

    // =========================
    // PLAYER TYPE
    // =========================
    IEnumerator TypePlayer()
    {
        isTyping = true;

        cameraManager?.SetCamera(CameraManager.CamState.PLAYER);

        textBubbleUI.ShowPlayerBubble();
        portraitUI.ShowPlayer(currentNode.playerPortrait);

        yield return TypeText(currentNode.playerText, ui.playerTextBox);

        isTyping = false;
    }

    // =========================
    // TYPE TEXT
    // =========================
    IEnumerator TypeText(string text, TMP_Text textBox)
    {
        textBox.text = "";

        foreach (char c in text)
        {
            textBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.25f);
    }

    // =========================
    // INPUT (CLICK TO CONTINUE)
    // =========================
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If typing → finish instantly
            if (isTyping)
            {
                StopTyping();
                return;
            }

            if (currentNode == null) return;

            // If NPC just finished → go to Player
            if (currentPhase == DialoguePhase.NPC &&
                !string.IsNullOrEmpty(currentNode.playerText))
            {
                currentPhase = DialoguePhase.PLAYER;
                typingRoutine = StartCoroutine(TypePlayer());
            }
            else
            {
                // Otherwise go to next node
                GoToNextNode();
            }
        }
    }

    // =========================
    // FLOW
    // =========================
    void GoToNextNode()
    {
        int next = currentNode.nextNode;

        if (next == -1)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        currentNodeIndex = next;
        DisplayNode();
    }

    public void ChooseOption(int option)
    {
        currentNodeIndex = (option == 1)
            ? currentNode.nextNodeForOption1
            : currentNode.nextNodeForOption2;

        DisplayNode();
    }

    // =========================
    // END
    // =========================
    IEnumerator EndDialogue()
    {
        textBubbleUI.HideAll();
        ui.HideChoices();

        cameraManager?.SetCamera(CameraManager.CamState.SCENE);

        // Re-enable player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pm = player.GetComponent<PlayerMovement>();
            if (pm) pm.enabled = true;
        }

        // Scene transition
        if (sceneLoader != null && !string.IsNullOrEmpty(nextSceneName))
        {
            yield return sceneLoader.LoadSceneWithTransition(nextSceneName);
        }
    }

    // =========================
    // HELPERS
    // =========================
    void StopTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        isTyping = false;

        // Instantly fill text
        if (currentPhase == DialoguePhase.NPC)
            ui.npcTextBox.text = currentNode.npcText;

        if (currentPhase == DialoguePhase.PLAYER)
            ui.playerTextBox.text = currentNode.playerText;
    }
}