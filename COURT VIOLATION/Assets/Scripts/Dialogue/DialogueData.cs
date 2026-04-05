using UnityEngine;
[System.Serializable]
public class DialogueNode
{
    
    [Header("Portraits")]
    public Sprite npcPortrait;
    public Sprite playerPortrait;

    public string npcID;

    [TextArea] public string npcText;
    [TextArea] public string playerText;

    public bool isChoiceNode;
    public string option1Text;
    public string option2Text;
    public int nextNodeForOption1;
    public int nextNodeForOption2;

    public int nextNode;
}
public class DialogueData : MonoBehaviour
{
    public DialogueNode[] nodes;

    public DialogueNode GetNode(int index) => nodes[index];

    public bool IsValidNode(int index)
    {
        return index >= 0 && index < nodes.Length;
    }
    
}

