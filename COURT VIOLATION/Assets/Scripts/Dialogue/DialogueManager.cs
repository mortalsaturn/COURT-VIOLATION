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
    private bool isWaitingForChoice = false;
    private Coroutine typingRoutine;

    enum DialoguePhase { NONE, NPC, PLAYER }
    private DialoguePhase currentPhase;
    private DialogueNode currentNode;

    void Start()
    {
        ui.option1Button.onClick.AddListener(() => ChooseOption(1));
        ui.option2Button.onClick.AddListener(() => ChooseOption(2));

        ui.HideChoices();
    }

    public void StartDialogue()
    {
        currentNodeIndex = 0;
        DisplayNode();
    }

    // DISPLAY NODE
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

        typingRoutine = StartCoroutine(TypeDialogue(currentNode));
    }

    IEnumerator TypeDialogue(DialogueNode node)
    {
        isTyping = true;

        // NPC
        if (!string.IsNullOrEmpty(node.npcText))
        {
            currentPhase = DialoguePhase.NPC;

            cameraManager?.SetCamera(CameraManager.CamState.NPC);

            textBubbleUI.ShowNPCBubble(node.npcID);
            portraitUI.ShowNPC(node.npcID, node.npcPortrait);

            TMP_Text npcBox = ui.GetNPCTextBox(node.npcID);
            if (npcBox != null)
                yield return TypeText(node.npcText, npcBox);
        }

        // PLAYER
        if (!string.IsNullOrEmpty(node.playerText))
        {
            currentPhase = DialoguePhase.PLAYER;

            cameraManager?.SetCamera(CameraManager.CamState.PLAYER);

            textBubbleUI.ShowPlayerBubble();
            portraitUI.ShowPlayer(node.playerPortrait);

            yield return TypeText(node.playerText, ui.playerTextBox);
        }

        isTyping = false;

        Debug.Log("NODE FINISHED");

        // SHOW CHOICES
        if (node.isChoiceNode)
        {
            Debug.Log("SHOWING CHOICES");

            isWaitingForChoice = true;
            ui.ShowChoices(node);
        }
    }

    // TYPE TEXT
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

    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || isTyping || isWaitingForChoice)
            return;

        if (currentNode == null)
            return;

        GoToNextNode();
    }

    // NEXT NODE
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

    // CHOICE BUTTONS
    public void ChooseOption(int option)
    {
        ui.HideChoices();

        int next = (option == 1)
            ? currentNode.nextNodeForOption1
            : currentNode.nextNodeForOption2;

        Debug.Log("CHOICE -> " + next);

        currentNodeIndex = next;

        StartCoroutine(ChoiceDelay());
    }

    IEnumerator ChoiceDelay()
    {
        yield return null; // wait one frame so the click doesn't bleed into Update()
        isWaitingForChoice = false;
        DisplayNode();
    }
    // END
    IEnumerator EndDialogue()
    {
        textBubbleUI.HideAll();
        ui.HideChoices();

        cameraManager?.SetCamera(CameraManager.CamState.SCENE);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var pm = player.GetComponent<PlayerMovement>();

            if (pm)
                pm.enabled = true;
        }

        if (sceneLoader != null &&
            !string.IsNullOrEmpty(nextSceneName))
        {
            yield return sceneLoader.LoadSceneWithTransition(nextSceneName);
        }
    }

    // STOP TYPING
    void StopTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        isTyping = false;

        // Fill text instantly
        if (currentPhase == DialoguePhase.NPC && currentNode != null)
        {
            TMP_Text npcBox = ui.GetNPCTextBox(currentNode.npcID);
            if (npcBox != null)
                npcBox.text = currentNode.npcText;
        }

        if (currentPhase == DialoguePhase.PLAYER)
            ui.playerTextBox.text = currentNode?.playerText;
    }
}