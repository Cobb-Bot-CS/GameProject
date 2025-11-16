using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * filename: PauseManager.cs
 * Developer: Adam Cobb
 * Purpose: Handles pausing, resuming, and game state transitions with audio integration
 */

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;

    /*
     * Summary: Called every frame to detect player input for pausing or resuming
     */
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /*
     * Summary: Resumes the game by hiding the pause menu and restoring time flow
     */
    public void ResumeGame()
    {
        AudioManager.Instance.PlayOneShot("ButtonClick");

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        else
            Debug.LogWarning("PauseMenuUI not found");

        Time.timeScale = 1f;
        isPaused = false;
    }

    /*
     * Summary: Pauses the game by displaying the pause menu and freezing time
     */
    private void PauseGame()
    {
        AudioManager.Instance.PlayOneShot("PauseOpen");

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        else
            Debug.LogWarning("PauseMenuUI reference not set in Inspector.");

        Time.timeScale = 0f;
        isPaused = true;
    }

    /*
     * Summary: Loads the Main Menu scene and resumes time
     */
    public void LoadMainMenu()
    {
        AudioManager.Instance.PlayOneShot("ButtonClick");

        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
            SceneManager.LoadScene("MainMenu");
        else
            Debug.LogError("MainMenu scene not found! Ensure it's added to Build Settings.");
    }

    /*
     * Summary: Restarts the current scene
     */
    public void RestartGame()
    {
        AudioManager.Instance.PlayOneShot("ButtonClick");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /*
     * Summary: Exits the application with audio feedback
     */
    public void QuitGame()
    {
        AudioManager.Instance.PlayOneShot("ButtonClick");

        Debug.Log("Quitting Game...");

        Invoke(nameof(QuitApplication), 0.5f);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
