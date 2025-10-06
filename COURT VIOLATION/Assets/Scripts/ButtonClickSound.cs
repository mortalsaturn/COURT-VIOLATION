using UnityEngine;
using UnityEngine.UI;

public class UIButtonSoundManager : MonoBehaviour
{
    public AudioSource audioSource;   // Shared AudioSource for all buttons
    public AudioClip clickSound;      // The sound effect for button clicks

    void Start()
    {
        // Find all buttons in the scene
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button btn in buttons)
        {
            // Add a listener to each button
            btn.onClick.AddListener(() => PlayClickSound());
        }
    }

    void PlayClickSound()
    {
        if (audioSource && clickSound)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
