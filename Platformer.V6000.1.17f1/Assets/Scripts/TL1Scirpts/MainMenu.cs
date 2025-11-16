/*
 * File: MainMenu.cs
 * Description: Handles main menu actions including starting the game and quitting.
 * Author: Adam Cobb
 * Date: 10-26-2025
 */

using UnityEngine;

/// 
/// Controls the main menu interactions such as starting or quitting the game.
/// 
/// 
/// Member Variables:
///    levelManager - reference to the LevelManager for scene loading.
/// 
public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    ///
    /// Starts the game by loading the first level.
    /// 
    public void PlayGame()
    {
      LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();

        if (levelManager != null)
        {
           
            levelManager.LoadLevel(100, 0);  // loading first level
        }
        else
        {
            Debug.LogError("No LevelManager found");
        }
    }

    /// 
    /// Quits the game and logs the action to the console.
    /// 
    public void QuitGame()
    {
        Debug.Log("[MainMenu] Game Quit!");
        Application.Quit();
    }
}
