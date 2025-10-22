using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BoulderStressTest
{
    private int boulderCount = 1;
    private GameObject orgBoulderObject;
    private GameObject newBoulderObject;
    private float yPos = -0.1f;

    [OneTimeSetUp]
    public void Setup()
    {
        SceneManager.LoadScene("TestingScene");
    }

    [UnityTest]
    public IEnumerator MaxBoulderStack()
    {
        // get the enemy object to duplicate
        orgBoulderObject = GameObject.Find("BoulderMoveable");

        // while bottom boulder stays above ground
        while (orgBoulderObject.transform.position.y > -0.18)
        {
            Debug.Log("Current boulder count:" + boulderCount);
            // wait for gravity to settle stack
            for (int i = 0; i < 50; i++)
            {
                yield return null;
            }
            yPos += 1f;
            newBoulderObject = Object.Instantiate(orgBoulderObject, new Vector3(3.5f, yPos, 0), Quaternion.identity);
            boulderCount++;

        }
        Debug.Log("Max boulder count:" + boulderCount);
    }
}