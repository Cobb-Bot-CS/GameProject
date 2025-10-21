using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class EnemyStressTest
{
    private int enemyCount = 1;
    private GameObject enemyObject;
    private float xPos = 6f;

    [OneTimeSetUp]
    public void Setup()
    {
        SceneManager.LoadScene("TestingScene");
    }

    [UnityTest]
    public IEnumerator EnemyStressTest_Continuous()
    {
        // get the enemy object to duplicate
        enemyObject = GameObject.Find("EnemyParent");

        // Wait for a few frames to stabilize the framerate
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }

        // while at least 60 fps
        while (1 / Time.deltaTime > 60)
        {
            yield return null;
            xPos -= 0.05f;
            enemyObject = Object.Instantiate(enemyObject, new Vector3(xPos, -0.1f, 0), Quaternion.identity);
            enemyCount++;

        }
        Debug.Log("Max enemy count:");
        Debug.Log(enemyCount);
    }
}
