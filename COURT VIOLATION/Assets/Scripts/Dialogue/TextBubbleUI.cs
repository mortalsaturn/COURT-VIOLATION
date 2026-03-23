using System.Collections.Generic;
using UnityEngine;

public class TextBubbleUI : MonoBehaviour
{
    public GameObject playerBubble;

    // Multiple NPC bubbles
    public List<NPCBubble> npcBubbles;

    private Dictionary<string, GameObject> npcBubbleMap;

    void Awake()
    {
        npcBubbleMap = new Dictionary<string, GameObject>();

        foreach (var npc in npcBubbles)
        {
            npcBubbleMap[npc.npcID] = npc.bubbleObject;
        }
    }

    public void ShowNPCBubble(string npcID)
    {
        HideAll();

        if (npcBubbleMap.ContainsKey(npcID))
            npcBubbleMap[npcID].SetActive(true);
    }

    public void ShowPlayerBubble()
    {
        HideAll();
        playerBubble.SetActive(true);
    }

    public void HideAll()
    {
        playerBubble.SetActive(false);

        foreach (var bubble in npcBubbleMap.Values)
            bubble.SetActive(false);
    }
}

[System.Serializable]
public class NPCBubble
{
    public string npcID;
    public GameObject bubbleObject;
}
