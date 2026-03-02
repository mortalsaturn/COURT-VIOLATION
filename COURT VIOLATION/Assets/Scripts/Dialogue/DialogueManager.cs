using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

#region DATA
[System.Serializable]
public class DialogueNode
{
    [TextArea(2, 5)] public string npcText;
    [TextArea(2, 5)] public string playerText;

    [Header("Portraits")]
    public Sprite npcPortrait;
    public Sprite playerPortrait;

    [Header("Choices")]
    public bool isChoiceNode;
    public string option1Text;
    public string option2Text;
    public int nextNodeForOption1 = -1;
    public int nextNodeForOption2 = -1;

    [Header("Flow")]
    public int nextNode = -1;
}
#endregion

public class DialogueManager : MonoBehaviour
{
    #region REFERENCES
    [Header("Scene Transition")]
    public SceneLoader sceneLoader;
    public string nextSceneName;

    [Header("Text Bubbles")]
    public GameObject npcBubble;
    public GameObject playerBubble;

    [Header("Portrait UI")]
    public Image npcPortraitImage;
    public Image playerPortraitImage;

    [Header("Camera Control")]
    public CameraManager cameraManager;

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
    #endregion

    #region STATE
    private int currentNodeIndex;
    private bool isTyping;
    private Coroutine typingRoutine;
    #endregion

    #region UNITY
    void Start()
    {
        option1Button.onClick.AddListener(() => ChooseOption(1));
        option2Button.onClick.AddListener(() => ChooseOption(2));
        HideChoices();
        HideAllVisuals();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping && !CurrentNode.isChoiceNode)
        {
            GoToNextNode();
        }
    }
    #endregion

    #region CORE FLOW
    public void StartDialogue()
    {
        currentNodeIndex = 0;
        DisplayNode();
        cameraManager?.SetCamera(CameraManager.CamState.SCENE);
    }

    void DisplayNode()
    {
        if (!IsValidNode())
        {
            StartCoroutine(EndDialogue());
            return;
        }

        StopTyping();
        ClearText();
        HideChoices();
        HideAllVisuals();

        typingRoutine = StartCoroutine(TypeDialogue(CurrentNode));
    }

    IEnumerator TypeDialogue(DialogueNode node)
    {
        isTyping = true;

        if (!string.IsNullOrEmpty(node.npcText))
            yield return TypeSpeaker(
                node.npcText,
                npcTextBox,
                npcBubble,
                npcPortraitImage,
                node.npcPortrait,
                CameraManager.CamState.NPC
            );

        if (!string.IsNullOrEmpty(node.playerText))
            yield return TypeSpeaker(
                node.playerText,
                playerTextBox,
                playerBubble,
                playerPortraitImage,
                node.playerPortrait,
                CameraManager.CamState.PLAYER
            );

        isTyping = false;

        if (node.isChoiceNode)
        {
            cameraManager?.SetCamera(CameraManager.CamState.SCENE);
            ShowChoices(node);
        }
    }
    #endregion

    #region HELPERS
    IEnumerator TypeSpeaker(
        string text,
        TMP_Text textBox,
        GameObject bubble,
        Image portraitImage,
        Sprite portrait,
        CameraManager.CamState camState
    )
    {
        cameraManager?.SetCamera(camState);

        SetBubble(bubble);
        SetPortrait(portraitImage, portrait);

        textBox.text = "";

        foreach (char c in text)
        {
            textBox.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.25f);
    }

    void SetBubble(GameObject activeBubble)
    {
        if (npcBubble) npcBubble.SetActive(activeBubble == npcBubble);
        if (playerBubble) playerBubble.SetActive(activeBubble == playerBubble);
    }

    void SetPortrait(Image image, Sprite sprite)
    {
        if (!image) return;

        image.sprite = sprite;
        image.gameObject.SetActive(sprite != null);

        if (image == npcPortraitImage && playerPortraitImage)
            playerPortraitImage.gameObject.SetActive(false);

        if (image == playerPortraitImage && npcPortraitImage)
            npcPortraitImage.gameObject.SetActive(false);
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

    void HideAllVisuals()
    {
        if (npcBubble) npcBubble.SetActive(false);
        if (playerBubble) playerBubble.SetActive(false);
        if (npcPortraitImage) npcPortraitImage.gameObject.SetActive(false);
        if (playerPortraitImage) playerPortraitImage.gameObject.SetActive(false);
    }

    void ClearText()
    {
        npcTextBox.text = "";
        playerTextBox.text = "";
    }

    void StopTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        isTyping = false;
    }

    void GoToNextNode()
    {
        int next = CurrentNode.nextNode;
        if (next == -1)
        {
            StartCoroutine(EndDialogue());
            return;
        }

        currentNodeIndex = next;
        DisplayNode();
    }

    void ChooseOption(int option)
    {
        currentNodeIndex = option == 1
            ? CurrentNode.nextNodeForOption1
            : CurrentNode.nextNodeForOption2;

        DisplayNode();
    }

    IEnumerator EndDialogue()
    {
        HideAllVisuals();
        HideChoices();

        cameraManager?.SetCamera(CameraManager.CamState.SCENE);

        PlayerMovement pm = GameObject.FindGameObjectWithTag("Player")
            ?.GetComponent<PlayerMovement>();

        if (pm) pm.enabled = true;

        if (sceneLoader && !string.IsNullOrEmpty(nextSceneName))
            yield return sceneLoader.LoadSceneWithTransition(nextSceneName);
    }

    bool IsValidNode()
    {
        return currentNodeIndex >= 0 && currentNodeIndex < dialogueNodes.Length;
    }

    DialogueNode CurrentNode => dialogueNodes[currentNodeIndex];
    #endregion
}