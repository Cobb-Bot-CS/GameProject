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
    private float speedMultiplier = 0.5f; // reduce delay by 50% each iteration
    private int maxIterations = 20;       // safety limit

    [UnityTest]
    public IEnumerator StressTest_PlayButtonBreakingPoint()
    {
        Debug.Log("[Stress Test] Starting Play button breaking point test...");

        for (int i = 0; i < maxIterations; i++)
        {
            pressCounter++;

            // --- Load Game Scene ---
            AsyncOperation loadGame = SceneManager.LoadSceneAsync("TestScene");
            loadGame.allowSceneActivation = true;
            while (!loadGame.isDone)
                yield return null;

            Assert.AreEqual("TestScene", SceneManager.GetActiveScene().name,
                $"[Stress Test] Failed to load TestScene on attempt #{pressCounter}");

            yield return new WaitForSeconds(speed);

            // --- Load Main Menu Scene ---
            AsyncOperation loadMenu = SceneManager.LoadSceneAsync("MainMenu");
            loadMenu.allowSceneActivation = true;
            while (!loadMenu.isDone)
                yield return null;

            Assert.AreEqual("MainMenu", SceneManager.GetActiveScene().name,
                $"[Stress Test] Failed to load MainMenu on attempt #{pressCounter}");

            yield return new WaitForSeconds(speed);

            // Reduce delay slightly each iteration
            speed *= speedMultiplier;
        }
        [SetUp]
public void DisableAudioManagerForTests()
{
    AudioManager audioManager = Object.FindObjectOfType<AudioManager>();
    if (audioManager != null)
        audioManager.gameObject.SetActive(false);
}
public void Load()
{
    if (someReference == null)
        return;
    // rest of load logic
}

        Debug.Log($"[Stress Test] Test completed {pressCounter} Play presses successfully!");
        Assert.Greater(pressCounter, 0, "Stress test did not complete any Play presses.");
    }
}
*/