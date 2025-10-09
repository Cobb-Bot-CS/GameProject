using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    public void PlayGame()
    {
        levelManager.LoadLevel(100,101);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}


