/*
 * Filename: LevelManager.cs
 * Developer: Alex Johnson
 * Purpose: A class that can switch scenes to change levels
 */ 

using UnityEngine;
using UnityEngine.SceneManagement;


/*
 * Summary: A class that can switch scenes to change levels
 * 
 * Member Variables:
 *    player: a gameobject to store the player so its position can be changed
 *    newPosition: Where to spawn the player in the next level
 */
public class LevelManager : MonoBehaviour
{
   private Vector3 newPosition;


   /*
    * Summary: Prevent gameobject from being destroyed on new scene
    */
   private void Awake()
   {
         DontDestroyOnLoad(gameObject);
   }


   /*
    * Summary: Loads the next level and saves the correct player position based on the current level
    * 
    * Parameters:
    *    nextLevel - The level you want to go to; ex. 100 for level 100, 0 for main menu
    *    currentLevel - The level you are comming from
    */
   public void LoadLevel(int nextLevel, int currentLevel)
   {
      switch (nextLevel)
      {
         case 100:
            if (currentLevel == 101)
            {
               newPosition = new Vector3(67f, 4.75f, 0f);
            }
            else
            {
               newPosition = new Vector3(0f, -0.25f, 0f);
            }
            SceneManager.LoadScene("Level100(Alex)");
            break;
         case 101:
            newPosition = new Vector3(0f, -0.25f, 0.0f);
            SceneManager.LoadScene("Boss1");
            break;
         case 102:
            SceneManager.LoadScene("Level102");
            break;
         default:
            SceneManager.LoadScene("MainMenu");
            break;
      }
   }


   public Vector3 GetSpawnPos()
   {
      return newPosition;
   }
}