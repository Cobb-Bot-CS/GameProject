/*
 * File: PauseManager.cs
 * Description: Handles pausing, resuming, and game state transitions (Pause Menu, Main Menu, Quit).
 * Author: Adam Cobb
 * Date: 10-29-2025
 */

using UnityEngine;
using UnityEngine.SceneManagement;

///
/// Manages the pause system for the game.
/// Detects player input to pause/resume, and provides UI control for scene navigation.
///
/// Member Variables:
///    pauseMenuUI - Reference to the Pause Menu Canvas object.
///    isPaused - Indicates whether the game is currently paused.
///
public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    public GameObject pauseMenuUI;   // Reference to Pause Menu Canvas
    private bool isPaused = false;   // Tracks current pause state

    ///
    /// Called every frame to detect player input for pausing or resuming.
    ///
    void Update()
    {
        // Check for Escape key press to toggle pause state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    ///
    /// Resumes the game by hiding the pause menu and restoring time flow.
    ///
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide pause menu UI
        Time.timeScale = 1f;          // Resume normal time
        isPaused = false;             // Update state flag
    }

    ///
    /// Pauses the game by displaying the pause menu and freezing time.
    ///
    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);  // Show pause menu UI
        Time.timeScale = 0f;          // Freeze all time-dependent systems
        isPaused = true;              // Update state flag
    }

    ///
    /// Loads the Main Menu scene and ensures time resumes normally.
    ///
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;                   // Ensure time is unfrozen
        SceneManager.LoadScene("MainMenu");    // Load main menu scene (adjust name if needed)
    }

    ///
    /// Exits the application.
    ///
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");  // Log message for debug
        Application.Quit();             // Quit the built application
    }
}
