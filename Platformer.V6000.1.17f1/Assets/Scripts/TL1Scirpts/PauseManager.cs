using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        // Check for Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                PlayGame();
            else
                Pause();
        }
    }

    public void PlayGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze time
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Unfreeze before loading
        SceneManager.LoadScene("MainMenu"); // Make sure this matches your menu scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
