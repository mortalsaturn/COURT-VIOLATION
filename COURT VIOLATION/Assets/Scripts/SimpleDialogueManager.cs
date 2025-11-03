using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class SimpleDialogueLine
{
    public string speakerName;
    [TextArea(2, 5)]
    public string dialogueText;
}
public class SimpleDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    [Header("Dialogue Data")]
    public SimpleDialogueLine[] lines;
    public float typingSpeed = 0.03f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingRoutine;

    void Start()
    {
        DisplayLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            GoToNextLine();
        }
    }

    void DisplayLine()
    {
        if (currentLineIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        SimpleDialogueLine line = lines[currentLineIndex];

        speakerNameText.text = line.speakerName;
        dialogueText.text = "";

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(line.dialogueText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void GoToNextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    void EndDialogue()
    {
        dialogueText.text = "";
        speakerNameText.text = "";
        Debug.Log("Dialogue finished.");
        // Optionally disable UI panel here
        // gameObject.SetActive(false);
    }
}
