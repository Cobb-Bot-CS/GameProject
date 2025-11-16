using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * filename: PauseManager.cs
 * Developer: Adam Cobb
 * Purpose: Handles pausing, resuming, and game state transitions with audio integration
 */

/*
 * Summary: Manages the pause system for the game with audio feedback
 *
 * Member Variables:
 * pauseMenuUI - Reference to the Pause Menu Canvas object
 * isPaused - Indicates whether the game is currently paused
 */
public class PauseManager : MonoBehaviour
{
   [Header("Pause Menu Settings")]
   [SerializeField] private GameObject pauseMenuUI;
   private bool isPaused = false;


    ///
    /// Resumes the game by hiding the pause menu and restoring time flow.
    ///
    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Hide pause menu UI
        }
        else
        {
            Debug.LogWarning("PauseMenuUI not found");
        }

   /*
    * Summary: Called every frame to detect player input for pausing or resuming
    */
   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
         if (isPaused)
         {
            ResumeGame();
         }
         else
         {
            PauseGame();
         }
      }
   }

    ///
    /// Pauses the game by displaying the pause menu and freezing time.
    ///
    private void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);  // Show pause menu UI
        }
        else
        {
            Debug.LogWarning("PauseMenuUI not found");
        }

        Time.timeScale = 0f;              // Freeze all  systems
        isPaused = true;                  // Update state flag
    }

    ///
    /// Loads the Main Menu scene and ensures time resumes normally.
    ///
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 

        // Safely check if the scene exists before loading
        if (Application.CanStreamedLevelBeLoaded("MainMenu"))
        {
            SceneManager.LoadScene("MainMenu"); 
        else
        {
            Debug.LogError("MainMenu scene not found.");
        }
    }

      Time.timeScale = 1f;
      isPaused = false;
   }



   /*
    * Summary: Pauses the game by displaying the pause menu and freezing time
    */
   private void PauseGame()
   {
      // Play pause sound
      AudioManager.Instance.PlayOneShot("PauseOpen");

      if (pauseMenuUI != null)
      {
         pauseMenuUI.SetActive(true);
      }
      else
      {
         Debug.LogWarning("PauseMenuUI reference not set in Inspector.");
      }

      Time.timeScale = 0f;
      isPaused = true;
   }



   /*
    * Summary: Loads the Main Menu scene and ensures time resumes normally
    */
   public void LoadMainMenu()
   {
      // Play button click sound
      AudioManager.Instance.PlayOneShot("ButtonClick");

      Time.timeScale = 1f;

      if (Application.CanStreamedLevelBeLoaded("MainMenu"))
      {
         SceneManager.LoadScene("MainMenu");
      }
      else
      {
         Debug.LogError("MainMenu scene not found! Ensure it's added to Build Settings.");
      }
   }



   /*
    * Summary: Restarts the current scene
    */
   public void RestartGame()
   {
      // Play button click sound
      AudioManager.Instance.PlayOneShot("ButtonClick");

      Time.timeScale = 1f;
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   }



   /*
    * Summary: Exits the application with audio feedback
    */
   public void QuitGame()
   {
      // Play button click sound
      AudioManager.Instance.PlayOneShot("ButtonClick");

      Debug.Log("Quitting Game...");

      // Optional: Add a delay for sound to play before quitting
      Invoke(nameof(QuitApplication), 0.5f);
   }



   /*
    * Summary: Actually quits the application (called via Invoke)
    */
   private void QuitApplication()
   {
      Application.Quit();
   }
}