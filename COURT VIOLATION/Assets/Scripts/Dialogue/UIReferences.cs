using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReferences : MonoBehaviour
{
    public TMP_Text npcTextBox;
    public TMP_Text playerTextBox;

    public Button option1Button;
    public Button option2Button;

    public TMP_Text option1Text;
    public TMP_Text option2Text;

    public void ShowChoices(DialogueNode node)
    {
        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);

        option1Text.text = node.option1Text;
        option2Text.text = node.option2Text;
    }

    public void HideChoices()
    {
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
    }
}