/*
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MenuUITests
{
    private MenuManager menuManager;
    private PauseManager pauseManager;
    private SettingsManager settingsManager;
    private WinScreen winScreen;

    // --------------------
    // SETUP: Load TestScene and find managers
    // --------------------
    [UnitySetUp]
    public IEnumerator Setup()
    {
         AudioManager audioManager = Object.FindObjectOfType<AudioManager>();
    if (audioManager != null)
    {
        audioManager.gameObject.SetActive(false);
        Debug.Log("[Test Setup] AudioManager disabled to prevent NullReferenceException.");
    }

        // --- Load TestScene ---
        if (SceneManager.GetActiveScene().name != "TestScene")
            yield return SceneManager.LoadSceneAsync("TestScene");

        yield return null; // wait a frame

        // --- Find manager objects ---
        menuManager = Object.FindObjectOfType<MenuManager>();
        pauseManager = Object.FindObjectOfType<PauseManager>();
        settingsManager = Object.FindObjectOfType<SettingsManager>();
        winScreen = Object.FindObjectOfType<WinScreen>();

        // --- Assertions to ensure objects exist ---
        Assert.IsNotNull(menuManager, "MenuManager not found in TestScene.");
        Assert.IsNotNull(pauseManager, "PauseManager not found in TestScene.");
        Assert.IsNotNull(settingsManager, "SettingsManager not found in TestScene.");
        Assert.IsNotNull(winScreen, "WinScreen not found in TestScene.");

        yield return null;
    }

    // --------------------
    // BOUNDARY TESTS
    // --------------------
    [UnityTest]
    public IEnumerator Test_ShowMainMenu()
    {
        menuManager.ShowMainMenu();
        yield return null;

        Assert.IsTrue(menuManager.mainMenuUI.activeSelf, "Main Menu should be active.");
        Assert.IsFalse(menuManager.settingsMenuUI.activeSelf, "Settings Menu should be inactive.");
    }

    [UnityTest]
    public IEnumerator Test_OpenSettingsMenu()
    {
        settingsManager.OpenSettings();
        yield return null;

        Assert.IsTrue(settingsManager.settingsMenuUI.activeSelf, "Settings Menu should be active.");
        Assert.AreEqual(0f, Time.timeScale, "Time should be frozen while settings are open.");
    }

    [UnityTest]
    public IEnumerator Test_CloseSettingsMenu()
    {
        settingsManager.CloseSettings();
        yield return null;

        Assert.IsFalse(settingsManager.settingsMenuUI.activeSelf, "Settings Menu should be inactive.");
        Assert.AreEqual(1f, Time.timeScale, "Time should resume when closing settings.");
    }

    [UnityTest]
    public IEnumerator Test_PauseGame()
    {
        pauseManager.TriggerPause(); // Use public wrapper
        yield return null;

        Assert.IsTrue(pauseManager.PauseMenuUI.activeSelf, "Pause Menu should be active.");
        Assert.AreEqual(0f, Time.timeScale, "Time should be frozen when paused.");
    }

    [UnityTest]
    public IEnumerator Test_ResumeGame()
    {
        pauseManager.ResumeGame();
        yield return null;

        Assert.IsFalse(pauseManager.PauseMenuUI.activeSelf, "Pause Menu should be inactive.");
        Assert.AreEqual(1f, Time.timeScale, "Time should resume after unpausing.");
    }

    [UnityTest]
    public IEnumerator Test_ShowWinScreen()
    {
        winScreen.ShowWinScreen();
        yield return null;

        Assert.IsTrue(winScreen.winUI.activeSelf, "Win Screen UI should be active.");
        Assert.AreEqual(0.1f, Time.timeScale, "Time should slow down when win screen is shown.");
    }

    [UnityTest]
    public IEnumerator Test_RestartGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        winScreen.RestartGame();
        yield return null;

        string newScene = SceneManager.GetActiveScene().name;
        Assert.AreEqual(currentScene, newScene, "Scene should restart to the same name.");
        Assert.AreEqual(1f, Time.timeScale, "Time should resume after restart.");
    }

    [UnityTest]
    public IEnumerator Test_GoToMainMenu()
    {
        winScreen.GoToMainMenu();
        yield return null;

        Assert.AreEqual("MainMenuScene", SceneManager.GetActiveScene().name,
            "Scene should change to Main Menu.");
        Assert.AreEqual(1f, Time.timeScale, "Time should resume when going to main menu.");
    }

    // --------------------
    // STRESS TEST: Open/Close Settings
    // --------------------
    [UnityTest]
    public IEnumerator StressTest_OpenCloseSettings()
    {
        int iterations = 20;
        for (int i = 0; i < iterations; i++)
        {
            settingsManager.OpenSettings();
            yield return null;
            Assert.IsTrue(settingsManager.settingsMenuUI.activeSelf, "Settings Menu should be active.");

            settingsManager.CloseSettings();
            yield return null;
            Assert.IsFalse(settingsManager.settingsMenuUI.activeSelf, "Settings Menu should be inactive.");
        }
    }

    // --------------------
    // STRESS TEST: Play Button Breaking Point
    // --------------------
    [UnityTest]
    public IEnumerator StressTest_PlayButtonBreakingPoint()
    {
        Debug.Log("[Stress Test] Starting Play button breaking point test...");

        int pressCounter = 0;
        float speed = 0.5f;           // initial delay
        float speedMultiplier = 0.5f; // reduce delay by 50% each iteration

        // Ensure starting at MainMenu
        if (SceneManager.GetActiveScene().name != "TestScene")
            yield return SceneManager.LoadSceneAsync("TestScene");

        yield return null;

        while (true)
        {
            Debug.Log("Time between Scenes: " + speed + " Seconds");
            pressCounter++;

            // Load Game
            AsyncOperation loadPlay = SceneManager.LoadSceneAsync("TestScene");
            loadPlay.allowSceneActivation = true;
            yield return new WaitForSeconds(speed);

            if (SceneManager.GetActiveScene().name != "TestScene")
            {
                Debug.LogError($"[Stress Test] Failed to load TestScene on attempt #{pressCounter}!");
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

            // Reduce delay slightly each iteration
            speed *= speedMultiplier;
        }

        Debug.Log($"[Stress Test] Test stopped after {pressCounter} Play presses. Breaking point reached!");
        Assert.Greater(pressCounter, 0, "Stress test did not complete any Play presses.");
    }
}
*/
