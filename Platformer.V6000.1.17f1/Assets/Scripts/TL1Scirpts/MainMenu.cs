using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
    }

    void Start()
    {

    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}


