using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        
        Handheld.Vibrate();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
        
        Handheld.Vibrate();
    }
    
    public void Credits()
    {
        SceneManager.LoadScene(2);
        
        Handheld.Vibrate();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        Handheld.Vibrate();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        Handheld.Vibrate();
    }

    public void Quit()
    {
        Debug.Log("Quiting the game...");
        
        Handheld.Vibrate();

        Application.Quit();
    }
}
