using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIReferences : MonoBehaviour
{
    [Header("NPC Text Boxes")]
    public List<NPCTextBox> npcTextBoxes; // assign all 3 in Inspector

    [Header("Player")]
    public TMP_Text playerTextBox;

    [Header("Choices")]
    public Button option1Button;
    public Button option2Button;
    public TMP_Text option1Text;
    public TMP_Text option2Text;

    private Dictionary<string, TMP_Text> npcTextBoxMap;

    void Awake()
    {
        npcTextBoxMap = new Dictionary<string, TMP_Text>();

        foreach (var entry in npcTextBoxes)
        {
            npcTextBoxMap[entry.npcID] = entry.textBox;
        }
    }

    // Call this from DialogueManager instead of ui.npcTextBox directly
    public TMP_Text GetNPCTextBox(string npcID)
    {
        if (npcTextBoxMap.ContainsKey(npcID))
            return npcTextBoxMap[npcID];

        Debug.LogWarning("No text box found for NPC ID: " + npcID);
        return null;
    }

    public void ShowChoices(DialogueNode node)
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);

        option1Text.gameObject.SetActive(true);
        option2Text.gameObject.SetActive(true);

        option1Text.text = node.option1Text;
        option2Text.text = node.option2Text;

        Debug.Log("CHOICES SHOWING: " + node.option1Text + " / " + node.option2Text);
    }

    public void HideChoices()
    {
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class NPCTextBox
{
    public string npcID;
    public TMP_Text textBox;
}