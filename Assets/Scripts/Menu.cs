﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToLevelSelection()
    {
        SceneManager.LoadScene(1);
    }
    
    public void Credits()
    {
        SceneManager.LoadScene(3);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber + 1);
    }

    public void Quit()
    {
        Debug.Log("Quiting the game...");

        Application.Quit();
    }
}
