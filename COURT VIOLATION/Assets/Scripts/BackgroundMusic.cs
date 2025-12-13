using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    [Header("Allowed Scenes")]
    public string[] allowedScenes;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (!IsSceneAllowed(scene.name))
        {
            Destroy(gameObject);
            instance = null;
        }
    }

    bool IsSceneAllowed(string sceneName)
    {
        foreach (string s in allowedScenes)
        {
            if (s == sceneName)
                return true;
        }
        return false;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

