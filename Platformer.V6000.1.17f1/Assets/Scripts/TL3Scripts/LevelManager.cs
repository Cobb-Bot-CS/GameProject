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
 */
public class LevelManager : MonoBehaviour
{
   //public static LevelManager instance;
   [SerializeField] GameObject player;

   /*
    * Summary: Gets the player object from the scene
    */
   private void Awake()
   {
      if (player == null)
      {
          player = GameObject.Find("CharacterParent");
      }
   }


   /*
    * Summary: Loads the next level and places the player in the correct position based on the current level
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
            SceneManager.LoadScene("Level100");
            break;
         case 101:
            SceneManager.LoadScene("Level101");
            if (player != null)
            {
               if (currentLevel == 100)
                  player.transform.position = new Vector3(0.5f, 0.65f, 0.0f);
               else
                  player.transform.position = new Vector3(3.0f, 0.65f, 0.0f);
            }
            break;
         case 102:
            SceneManager.LoadScene("Level102");
            break;
         default:
            SceneManager.LoadScene("MainMenu");
            break;
      }
   }


    // Temp fix, delete this later
   public void LoadLevel100()
   {
      LoadLevel(100, 0);
   }
}


/*
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
     public static LevelManager instance;
    [SerializeField] GameObject player;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.Find("CharacterParent");
        }
    }
    // maybe levels 1xx in first era, 2xx in second era, etc.
    // accepts positive integers
    // nextLevel is the level player is going to
    // currentLevel is the level player is leaving
    public void LoadLevel(int nextLevel, int currentLevel)
    {
        switch (nextLevel)
        {
            case 100:
                SceneManager.LoadScene("SceneV1");
                break;

            case 101:
                SceneManager.LoadScene("Level101");
                // place player in appropriate position
                if (player == null)
                {
                    if (currentLevel == 100)
                    {
                        player.transform.position = new Vector3(0.5f, 0.65f, 0.0f);
                    }
                    else
                    {
                        player.transform.position = new Vector3(3.0f, 0.65f, 0.0f);
                    }
                }
                break;

            case 102:
                SceneManager.LoadScene("Level102");
                break;

            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }
}
*/