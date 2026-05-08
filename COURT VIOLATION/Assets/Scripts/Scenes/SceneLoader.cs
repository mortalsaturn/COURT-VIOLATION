using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void GameScene()
    {
        SceneManager.LoadScene("Match Scene");
    }

    public void DialogueScenePro()
    {
        SceneManager.LoadScene("Prologue");
    }

    public void DialogueSceneDe()
    {
        SceneManager.LoadScene("Debrief");
    }

    public void DialogueScene1()
    {
        SceneManager.LoadScene("Scene 1");
    }
    public void DialogueScene2()
    {
        SceneManager.LoadScene("Scene 2");
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadSceneAsync(levelIndex);

        transition.SetTrigger("Start");
    }
    public IEnumerator LoadSceneWithTransition(string sceneName)
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }

}
