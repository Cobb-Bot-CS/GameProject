/*
 * Filename: LevelManagerTests.cs
 * Developer: Alex Johnson
 * Purpose: Boundary tests to make sure the LevelManager switches to the correct
 *          levels given a variety if inputs
 */

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/*
 * Summary: Boundary tests to make sure the LevelManager switches to the correct
 *          levels given a variety if inputs
 * 
 * Member Variables:
 *    levelManagerObject - GameObject to store the LevelManager object from the scene
 *    lm - LevelManager for the scripts from that class
 *    scene - Scene to get the name of scenes
 */
public class LevelManagerTests
{
   private GameObject levelManagerObject;
   private LevelManager lm;
   private Scene scene;


   /*
    * Summary: Load the main menu
    */
   [SetUp]
   public void Setup()
   {
      SceneManager.LoadScene("MainMenu");
   }


   /*
    * Summary: Load the main menu from a variety of other levels
    */
   [UnityTest]
   public IEnumerator LoadMainMenu()
   {
      // get the level manager script to use methods
      levelManagerObject = GameObject.Find("LevelManager");
      if (levelManagerObject != null)
      {
         lm = levelManagerObject.GetComponent<LevelManager>();
      }

      // main menu to main menu
      lm.LoadLevel(0, 0);
      // wait for scene to load
      yield return null;
      // get current scene
      scene = SceneManager.GetActiveScene();
      // make sure scene name is correct
      Assert.AreEqual("MainMenu", scene.name);

      // valid level to main menu
      lm.LoadLevel(0, 100);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("MainMenu", scene.name);
      // invalid level to main menu
      lm.LoadLevel(0, -2);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("MainMenu", scene.name);
   }


   /*
    * Summary: Load a valid level from a variety of other levels
    */
   [UnityTest]
   public IEnumerator LoadValidLevel()
   {
      levelManagerObject = GameObject.Find("LevelManager");
      if (levelManagerObject != null)
      {
         lm = levelManagerObject.GetComponent<LevelManager>();
      }

      // main menu to valid level
      lm.LoadLevel(100, 0);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("Level100(Alex)", scene.name);
      // valid level to valid level
      lm.LoadLevel(100, 101);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("Level100(Alex)", scene.name);
      // invalid level to valid level
      lm.LoadLevel(100, -2);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("Level100(Alex)", scene.name);
      // valid level to same level
      lm.LoadLevel(100, 100);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("Level100(Alex)", scene.name);
   }


   /*
    * Summary: Load an invalid level from a variety of other levels
    */
   [UnityTest]
   public IEnumerator LoadInvalidLevel()
   {
      // get the level manager script to use methods
      levelManagerObject = GameObject.Find("LevelManager");
      if (levelManagerObject != null)
      {
         lm = levelManagerObject.GetComponent<LevelManager>();
      }

      // main menu to invalid level
      lm.LoadLevel(-2, 0);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("MainMenu", scene.name);
      // valid level to invalid level
      lm.LoadLevel(-2, 100);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("MainMenu", scene.name);
      // invalid level to invalid level
      lm.LoadLevel(-2, -2);
      yield return null;
      scene = SceneManager.GetActiveScene();
      Assert.AreEqual("MainMenu", scene.name);
   }
}
