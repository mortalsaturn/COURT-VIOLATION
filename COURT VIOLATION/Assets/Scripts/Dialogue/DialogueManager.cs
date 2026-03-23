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

    // =========================
    // START
    // =========================
    public void StartDialogue()
    {
        currentNodeIndex = 0;
        DisplayNode();

        cameraManager?.SetCamera(CameraManager.CamState.SCENE);
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

        DialogueNode node = dialogueData.GetNode(currentNodeIndex);

        StopTyping();
        ui.HideChoices();
        textBubbleUI.HideAll();

        typingRoutine = StartCoroutine(TypeNode(node));
    }

    // =========================
    // TYPE NODE
    // =========================
    IEnumerator TypeNode(DialogueNode node)
    {
        isTyping = true;

        // NPC
        if (!string.IsNullOrEmpty(node.npcText))
        {
            cameraManager?.SetCamera(CameraManager.CamState.NPC);

            textBubbleUI.ShowNPCBubble(node.npcID);
            portraitUI.ShowNPC(node.npcID, node.npcPortrait);

            yield return TypeText(node.npcText, ui.npcTextBox);
        }

        // PLAYER
        if (!string.IsNullOrEmpty(node.playerText))
        {
            cameraManager?.SetCamera(CameraManager.CamState.PLAYER);

            textBubbleUI.ShowPlayerBubble();
            portraitUI.ShowPlayer(node.playerPortrait);

            yield return TypeText(node.playerText, ui.playerTextBox);
        }

        isTyping = false;

        // Choices
        if (node.isChoiceNode)
        {
            cameraManager?.SetCamera(CameraManager.CamState.SCENE);
            ui.ShowChoices(node);
        }
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
        if (isTyping) return;

        if (Input.GetMouseButtonDown(0))
        {
            DialogueNode node = dialogueData.GetNode(currentNodeIndex);

            if (!node.isChoiceNode)
            {
                GoToNextNode();
            }
        }
    }

    // =========================
    // FLOW
    // =========================
    void GoToNextNode()
    {
        int next = dialogueData.GetNode(currentNodeIndex).nextNode;

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
        DialogueNode node = dialogueData.GetNode(currentNodeIndex);

        currentNodeIndex = (option == 1)
            ? node.nextNodeForOption1
            : node.nextNodeForOption2;

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
    }
}