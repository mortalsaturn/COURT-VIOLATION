using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterDialogue
    {
        public string characterName;
        public GameObject bubbleObject; // Speech bubble above the character
        public TMP_Text dialogueText;   // TextMeshPro text inside the bubble
    }

    [Header("Characters")]
    public CharacterDialogue persephone;
    public CharacterDialogue lilith;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.03f; // Speed of the typewriter effect

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] dialogueLines; // Put your full conversation here

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingRoutine;

    void Start()
    {
        HideAllBubbles();
        if (dialogueLines.Length > 0)
        {
            ShowNextLine(); // Start conversation automatically
        }
    }

    void Update()
    {
        // Click or tap to advance
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Skip typing animation and show full line
                StopCoroutine(typingRoutine);
                FinishTyping();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void ShowNextLine()
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            HideAllBubbles();
            return;
        }

        string line = dialogueLines[currentLineIndex];
        currentLineIndex++;

        // Format: "Persephone: Hello there!"
        string[] splitLine = line.Split(':');
        if (splitLine.Length < 2)
        {
            Debug.LogWarning("Invalid line format. Use 'Character: Dialogue text'");
            return;
        }

        string speakerName = splitLine[0].Trim();
        string dialogueText = splitLine[1].Trim();

        CharacterDialogue currentSpeaker = null;
        if (speakerName.ToLower() == "persephone")
            currentSpeaker = persephone;
        else if (speakerName.ToLower() == "lilith")
            currentSpeaker = lilith;
        else
        {
            Debug.LogWarning("Unknown speaker name: " + speakerName);
            return;
        }

        // Hide the other bubble
        HideAllBubbles();
        currentSpeaker.bubbleObject.SetActive(true);
        currentSpeaker.dialogueText.text = "";

        // Typewriter effect
        typingRoutine = StartCoroutine(TypeLine(currentSpeaker.dialogueText, dialogueText));
    }

    IEnumerator TypeLine(TMP_Text textUI, string line)
    {
        isTyping = true;
        foreach (char c in line)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void FinishTyping()
    {
        isTyping = false;
        // Show the full current line instantly
        string line = dialogueLines[currentLineIndex - 1];
        string[] splitLine = line.Split(':');
        string fullText = splitLine.Length > 1 ? splitLine[1].Trim() : "";
        string speakerName = splitLine[0].Trim();

        CharacterDialogue currentSpeaker = 
            speakerName.ToLower() == "persephone" ? persephone : lilith;

        currentSpeaker.dialogueText.text = fullText;
    }

    void HideAllBubbles()
    {
        persephone.bubbleObject.SetActive(false);
        lilith.bubbleObject.SetActive(false);
    }
}
