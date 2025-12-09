using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Loop Times (in seconds)")]
    public float loopStart = 15f;
    public float loopEnd = 25f;

    void Start()
    {
        audioSource.time = loopStart;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource.time >= loopEnd)
        {
            audioSource.time = loopStart;
        }
    }
}
