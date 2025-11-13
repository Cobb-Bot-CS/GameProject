/*
 * Filename: LevelSwitcher.cs
 * Developer: Alex Johnson
 * Purpose: Enables a GameObject to use the LevelManager class to switch levels upon contact with the player
 */

using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Summary: A class that enables a GameObject to switch levels upon contact with the player
 * 
 * Member Variables:
 *    nextLevel - The level that will be switched to
 *    nextLevel - The level that is being switched off of
 *    levelManagerObject - A GameObject to hold the LevelManager
 *    levelManager - A LevelManager that we can use scripts from
 */
public class LevelSwitcher : MonoBehaviour
{
   [SerializeField] private int nextLevel;
   [SerializeField] private int currentLevel;

   [SerializeField] private GameObject levelManagerObject;
   private LevelManager levelManager;

   /*
    * Summary: Find the LevelManager object if needed and get the script from it
    */
   private void Awake()
   {
      // Make sure we have level manager gameobject
      if (levelManagerObject == null)
      {
         levelManagerObject = GameObject.Find("LevelManager");
      }
      // get the level manager script to use methods
      if (levelManagerObject != null)
      {
         levelManager = levelManagerObject.GetComponent<LevelManager>();
      }
   }


   /*
    * Summary: upon colliding with player switch the level
    * 
    * Parameters:
    *    other - the GameObject that has been collided with
    */
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (levelManager != null)
      {
         GameObject otherGameObject = other.gameObject;
         if (otherGameObject.CompareTag("Player"))
         {
            levelManager.LoadLevel(nextLevel, currentLevel);
         }
      }
      else
      {
         Debug.LogWarning("Level Switcher couldn't find level manager");
      }
   }
}
