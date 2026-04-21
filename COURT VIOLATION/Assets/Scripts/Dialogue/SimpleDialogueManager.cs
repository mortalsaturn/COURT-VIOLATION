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

    [Header("Scene Transition")]
    public SceneLoader sceneLoader;
    public string nextSceneName;

    [Header("Pause Menu")]
    public GameObject pausePanel; // drag your panel here

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isPaused = false;
    private Coroutine typingRoutine;

    void Start()
    {
        pausePanel.SetActive(false); // hide panel at start
        DisplayLine();
    }

    void Update()
    {
        // 🚨 Stop all dialogue input if paused
        if (isPaused) return;

        // Click to continue
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            GoToNextLine();
        }

        // Click to finish typing instantly
        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            StopTypingInstant();
        }
    }

    // =========================
    // BUTTON FUNCTIONS
    // =========================
    public void OpenMenu()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // freeze game
    }

    public void CloseMenu()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // resume game
    }

    // =========================
    // DISPLAY LINE
    // =========================
    void DisplayLine()
    {
        if (currentLineIndex >= lines.Length)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        SimpleDialogueLine line = lines[currentLineIndex];

        speakerNameText.text = line.speakerName;
        dialogueText.text = "";

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeText(line.dialogueText));
    }

    // =========================
    // TYPE TEXT
    // =========================
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

    // =========================
    // INSTANT FINISH
    // =========================
    void StopTypingInstant()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        isTyping = false;
        dialogueText.text = lines[currentLineIndex].dialogueText;
    }

    // =========================
    // FLOW
    // =========================
    void GoToNextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    // =========================
    // END
    // =========================
    IEnumerator EndDialogue()
    {
        dialogueText.text = "";
        speakerNameText.text = "";

        Debug.Log("Dialogue finished.");

        if (sceneLoader != null && !string.IsNullOrEmpty(nextSceneName))
        {
            yield return sceneLoader.LoadSceneWithTransition(nextSceneName);
        }
    }
}