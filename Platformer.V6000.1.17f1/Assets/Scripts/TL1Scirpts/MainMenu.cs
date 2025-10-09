using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        LevelManager.instance.StartLevelWithMenu(101); 
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}


