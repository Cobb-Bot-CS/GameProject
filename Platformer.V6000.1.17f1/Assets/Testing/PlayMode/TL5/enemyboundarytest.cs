using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
public class EnemyBoundaryTest
{
    private GameObject enemy;
    private GameObject enemy1;
    private GameObject enemy2;

    private EnemyMoveScript moveScript;
    private EnemyMoveScript moveScript1;
    private EnemyMoveScript moveScript2;




    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("TestingScene");
 
        //moveScript = enemy.AddComponent<EnemyMoveScript>();
    }

    [UnityTest]
    public IEnumerator EnemyDoesNotExceedBoundaries()
    {
        enemy = GameObject.Find("Enemy");
        enemy1 = GameObject.Find("Enemy (1)");
        enemy2 = GameObject.Find("Enemy (2)");

       for (int i = 0; i < 10; i++)
        {
            Debug.Log(enemy.transform.position.y);
            yield return new WaitForSeconds(.1f);

        }
        // Capture starting position
        float startY = enemy.transform.position.y;  

        // Wait a few frames to let it start moving
        yield return new WaitForSeconds(.05f);

        Assert.Greater(startY,enemy.transform.position.y);
        yield return null;
        // Capture starting position
         startY = enemy1.transform.position.y;

        // Wait a few frames to let it start moving
        yield return new WaitForSeconds(2f);

        Assert.AreEqual(startY, enemy1.transform.position.y);

        // Capture starting position
         startY = enemy2.transform.position.y;

        // Wait a few frames to let it start moving
        yield return new WaitForSeconds(2f);

        Assert.Less(startY, enemy2.transform.position.y);



      
    }
}
