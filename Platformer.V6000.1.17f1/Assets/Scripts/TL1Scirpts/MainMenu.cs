/*
 * File: MainMenu.cs
 * Description: Handles main menu actions including starting the game and quitting.
 * Author: Adam Cobb
 * Date: 10-27-2025
 */

using UnityEngine;


/// 
/// Controls the main menu interactions such as starting or quitting the game.
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
      // Use FindObjectOfType to get your LevelManager instance
      LevelManager levelManager = Object.FindAnyObjectByType<LevelManager>();

      if (levelManager != null)
      {
         // Load Level 100 (SceneV1)
         levelManager.LoadLevel(100, 0);   // 0 = no previous level
      }
      else
      {
         Debug.LogError("No LevelManager found in the scene!");
      }
   }


   /// 
   /// Quits the game and logs the action to the console.
   /// 
   public void QuitGame()
   {
      Application.Quit();
      Debug.Log("Game Quit!");
   }
}
