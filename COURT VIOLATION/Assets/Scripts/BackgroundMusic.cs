using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Keep only one music manager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Music continues across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates after loading scenes
        }
    }
}
