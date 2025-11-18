/*
 * Filename: BoulderStressTest.cs
 * Developer: Alex Johnson
 * Purpose: A stress test to determine how many objects can de stacked on top
 *          of each other before they start clipping into each other
 */

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/*
 * Summary: A stress test to determine how many objects can de stacked on top
 *          of each other before they start clipping into each other
 * 
 * Member Variables:
 *    boulderCount - Int to storethe current number of boulders
 *    orgBoulderObject - GameObject to store the original boulder in the scene
 *    orgBoulderObject - GameObject to store the new boulder created
 *    yPos - Y position to spawn the next boulder at
 */
public class BoulderStressTest
{
   private int boulderCount = 1;
   private GameObject orgBoulderObject;
   private GameObject newBoulderObject;
   private float yPos = 0f;


   /*
    * Summary: Load the testing scene
    */
   [OneTimeSetUp]
   public void Setup()
   {
      SceneManager.LoadScene("AlexTestScene");
   }


   /*
    * Summary: Repeatedly spawn boulders on top of each other until the bottom one
    *          clips into the ground, return the max ammount
    */
   [UnityTest]
   public IEnumerator MaxBoulderStack()
   {
      // get the enemy object to duplicate
      orgBoulderObject = GameObject.Find("BoulderMoveable (1)");

      // while bottom boulder stays above ground
      while (orgBoulderObject.transform.position.y > -0.05)
      {
         // Debug.Log("Boulder pos:" + orgBoulderObject.transform.position.y);
         Debug.Log("Current boulder count:" + boulderCount);
         // wait for gravity to settle stack
         yield return new WaitForSeconds(0.3f);
         yPos += 2f;
         newBoulderObject = Object.Instantiate(orgBoulderObject, new Vector3(-5f, yPos, 0), Quaternion.identity);
         boulderCount++;

      }
      Debug.Log("Max boulder count:" + boulderCount);
   }
}