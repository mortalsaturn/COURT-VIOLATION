using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    public Image playerPortrait;

    public List<NPCPortrait> npcPortraits;
    private Dictionary<string, Image> npcPortraitMap;

    void Awake()
    {
        npcPortraitMap = new Dictionary<string, Image>();

        foreach (var npc in npcPortraits)
        {
            npcPortraitMap[npc.npcID] = npc.image;
        }
    }

    public void ShowNPC(string npcID, Sprite sprite)
    {
        HideAll();

        if (npcPortraitMap.ContainsKey(npcID))
        {
            var img = npcPortraitMap[npcID];
            img.sprite = sprite;
            img.gameObject.SetActive(true);
        }
    }

    public void ShowPlayer(Sprite sprite)
    {
        HideAll();
        playerPortrait.sprite = sprite;
        playerPortrait.gameObject.SetActive(true);
    }

    void HideAll()
    {
        playerPortrait.gameObject.SetActive(false);

        foreach (var img in npcPortraitMap.Values)
            img.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class NPCPortrait
{
    public string npcID;
    public Image image;
}
