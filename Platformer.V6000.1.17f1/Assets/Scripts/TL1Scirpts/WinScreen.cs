using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public GameObject winUI; // Assign your Panel here

    private void Start()
    {
        if (winUI != null)
            winUI.SetActive(false); // Make sure it starts hidden
    }

    public void ShowWinScreen()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = .1f; // Freeze game
        }
        else
        {
            Debug.LogError("Win UI panel not assigned!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene"); // Replace with your main menu scene
    }
}
