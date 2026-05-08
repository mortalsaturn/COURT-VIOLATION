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
    public GameObject pausePanel;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isPaused = false;
    private bool isAdvanceLocked = false; // prevents click bleed from pause button
    private Coroutine typingRoutine;

    void Start()
    {
        pausePanel.SetActive(false);
        DisplayLine();
    }

    void Update()
    {
        if (isPaused) return;
        if (isAdvanceLocked) return;

        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            StopTypingInstant();
            return;
        }

        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            GoToNextLine();
        }
    }

    // =========================
    // BUTTON FUNCTIONS
    // =========================
    public void OpenMenu()
    {
        isPaused = true;
        isAdvanceLocked = true; // lock dialogue click for this frame
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        StartCoroutine(UnlockAfterFrame());
    }

    public void CloseMenu()
    {
        isPaused = false;
        isAdvanceLocked = true; // lock dialogue click for this frame
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        StartCoroutine(UnlockAfterFrame());
    }

    IEnumerator UnlockAfterFrame()
    {
        // WaitForSecondsRealtime works even when timeScale is 0
        yield return new WaitForSecondsRealtime(0f);
        isAdvanceLocked = false;
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
            yield return new WaitForSecondsRealtime(typingSpeed); // realtime so pause doesn't freeze typing mid-sentence
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