using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueNode
{
    [TextArea(2, 5)] public string npcText;
    [TextArea(2, 5)] public string playerText;

    public bool isChoiceNode;
    public string option1Text;
    public string option2Text;
    public int nextNodeForOption1 = -1;
    public int nextNodeForOption2 = -1;

    // For non-choice progression
    public int nextNode = -1;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text npcTextBox;
    public TMP_Text playerTextBox;
    public Button option1Button;
    public Button option2Button;
    public TMP_Text option1Text;
    public TMP_Text option2Text;

    [Header("Dialogue Data")]
    public DialogueNode[] dialogueNodes;
    public float typingSpeed = 0.03f;

    private int currentNodeIndex = 0;
    private bool isTyping = false;
    private bool npcFinished = false;
    private bool playerFinished = false;

    private Coroutine typingRoutine;

    void Start()
    {
        option1Button.onClick.AddListener(() => ChooseOption(1));
        option2Button.onClick.AddListener(() => ChooseOption(2));

        HideChoices();
        DisplayNode();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping && !dialogueNodes[currentNodeIndex].isChoiceNode)
            {
                GoToNextNode();
            }
        }
    }

    void DisplayNode()
    {
        DialogueNode node = dialogueNodes[currentNodeIndex];

        npcTextBox.text = "";
        playerTextBox.text = "";

        HideChoices();
        StopAllCoroutines();

        // Start the NPC typewriter first
        typingRoutine = StartCoroutine(TypeDialogue(node));
    }

    IEnumerator TypeDialogue(DialogueNode node)
    {
        // --- NPC dialogue ---
        isTyping = true;
        npcFinished = false;

        foreach (char c in node.npcText)
        {
            npcTextBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        npcFinished = true;
        yield return new WaitForSeconds(0.3f); // pause before player text

        // --- Player dialogue ---
        if (!string.IsNullOrEmpty(node.playerText))
        {
            playerFinished = false;
            foreach (char c in node.playerText)
            {
                playerTextBox.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            playerFinished = true;
        }

        isTyping = false;

        // If this node has choices, show them after typing finishes
        if (node.isChoiceNode)
        {
            ShowChoices(node);
        }
    }

    void GoToNextNode()
    {
        DialogueNode node = dialogueNodes[currentNodeIndex];

        if (node.isChoiceNode)
            return; // Wait for player input on choices

        if (node.nextNode != -1)
        {
            currentNodeIndex = node.nextNode;
            DisplayNode();
        }
        else
        {
            npcTextBox.text = "";
            playerTextBox.text = "";
            Debug.Log("Dialogue ended.");
        }
    }

    void ShowChoices(DialogueNode node)
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);

        option1Text.text = node.option1Text;
        option2Text.text = node.option2Text;
    }

    void HideChoices()
    {
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
    }

    void ChooseOption(int option)
    {
        DialogueNode node = dialogueNodes[currentNodeIndex];
        HideChoices();

        if (option == 1)
            currentNodeIndex = node.nextNodeForOption1;
        else
            currentNodeIndex = node.nextNodeForOption2;

        DisplayNode();
    }
}
