using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public GameObject LevelSelect;
    public GameObject MainMenu;
    public GameObject tutorialPage;
    public GameObject creditsPage;
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void TutorialPage(bool trueorfalse)
    {
        tutorialPage.SetActive(trueorfalse);
    }
    public void OptionsPage(bool trueorfalse)
    {
        creditsPage.SetActive(trueorfalse);
    }
    public void LoadSceneWithDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneAfterDelay(sceneName, delay));
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    public void OpenLevelSelect()
    {
        LevelSelect.SetActive(true);
        MainMenu.SetActive(false);
    }
   
}
