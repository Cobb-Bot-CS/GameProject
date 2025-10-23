using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class LevelManagerTests
{
    private GameObject levelManagerObject;
    private LevelManager lm;
    private Scene scene;

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("MainMenu");
    }


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
        Assert.AreEqual("SceneV1", scene.name);
        // valid level to valid level
        lm.LoadLevel(100, 101);
        yield return null;
        scene = SceneManager.GetActiveScene();
        Assert.AreEqual("SceneV1", scene.name);
        // invalid level to valid level
        lm.LoadLevel(100, -2);
        yield return null;
        scene = SceneManager.GetActiveScene();
        Assert.AreEqual("SceneV1", scene.name);
        // valid level to same level
        lm.LoadLevel(100, 100);
        yield return null;
        scene = SceneManager.GetActiveScene();
        Assert.AreEqual("SceneV1", scene.name);
    }

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
