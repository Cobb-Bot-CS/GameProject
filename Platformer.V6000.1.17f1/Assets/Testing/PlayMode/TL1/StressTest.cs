/*
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayButtonStressTest
{
    private int pressCounter = 0;
    private float speed = 0.5f;           // initial delay
    private float speedMultiplier = .5f; // reduce delay by 50% each iteration

    [UnityTest]
    public IEnumerator StressTest_PlayButtonBreakingPoint()
    {
         Debug.Log("[Stress Test] Starting Play button breaking point test...");

        // Ensure starting at MainMenu
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SceneManager.LoadScene("MainMenu");
        yield return null;

        while (true)
        {
            Debug.Log("Time between Scnenes " + speed + " Seconds");
            pressCounter++;

            // Load Game
            AsyncOperation loadPlay = SceneManager.LoadSceneAsync("TestingScene");
            loadPlay.allowSceneActivation = true;
            yield return new WaitForSeconds(speed);

            if (SceneManager.GetActiveScene().name != "TestingScene")
            {
                Debug.LogError($"[Stress Test] Failed to load SceneV1 on attempt #{pressCounter}!");
                break;
            }

            // Load MainMenu
            AsyncOperation loadMenu = SceneManager.LoadSceneAsync("MainMenu");
            loadMenu.allowSceneActivation = true;
            yield return new WaitForSeconds(speed);
        
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                Debug.LogError($"[Stress Test] Failed to return to MainMenu on attempt #{pressCounter}!");
                break;
            }

            // Reduce delay slightly each iteration to make it faster
            speed *= speedMultiplier;
           
        }

        Debug.Log($"[Stress Test] Test stopped after {pressCounter} Play presses. Breaking point reached!");
        Assert.Greater(pressCounter, 0, "Stress test did not complete any Play presses.");
    }
}

*/