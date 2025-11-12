using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class SceneLoader : MonoBehaviour

{
    public Animator transtion;
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
    public void DialogueScene1()
    {
        SceneManager.LoadScene("Prologue");
    }
    public void DialogueScene2()
    {
        SceneManager.LoadScene("Debrief");
    }
    public void DialogueScene3()
    {
        SceneManager.LoadScene("Scene 1");
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transtion.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
