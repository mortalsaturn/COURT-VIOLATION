using UnityEngine;
using UnityEngine.UI;

public class UIButtonSoundManager : MonoBehaviour
{

    public AudioSource source;
    public AudioClip clickClip;

    public void PlayClick()
    {
        source.PlayOneShot(clickClip);
    }
}
