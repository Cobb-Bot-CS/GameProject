using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    public void PlayGame()
    {
        // Use FindObjectOfType to get your LevelManager instance
        LevelManager levelManager = FindObjectOfType<LevelManager>();

        if (levelManager != null)
        {
            // Load Level 100 (SceneV1)
            levelManager.LoadLevel(100, 0); // 0 = no previous level
        }
        else
        {
            Debug.LogError("No LevelManager found in the scene!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}




