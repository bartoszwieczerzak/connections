using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
    }

    public void Play()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
    }

    public void Quit()
    {
        Debug.Log("Quiting the game...");

        Application.Quit();
    }
}
