/*
using System.Collections;
using UnityEngine;

public class MenuTestRunner : MonoBehaviour
{
    [Header("Menu References")]
    public MenuManager menuManager;
    public PauseManager pauseManager;
    public SettingsManager settingsManager;
    public WinScreen winScreen;

    // -------------------------------
    // Individual tests
    // -------------------------------

    [ContextMenu("Test 1 - Show Main Menu")]
    public void Test_ShowMainMenu()
    {
        Debug.Log("[Test] Show Main Menu");
        menuManager.ShowMainMenu();
    }

    [ContextMenu("Test 2 - Open Settings")]
    public void Test_OpenSettings()
    {
        Debug.Log("[Test] Open Settings Menu");
        settingsManager.OpenSettings();
    }

    [ContextMenu("Test 3 - Show Pause Menu")]
    public void Test_PauseGame()
    {
        Debug.Log("[Test] Pause Game");
        pauseManager.TriggerPause();
    }

    [ContextMenu("Test 4 - Show Win Screen")]
    public void Test_WinScreen()
    {
        Debug.Log("[Test] Show Win Screen");
        winScreen.ShowWinScreen();
    }

    [ContextMenu("Test 5 - Restart Game")]
    public void Test_RestartGame()
    {
        Debug.Log("[Test] Restart Game");
        pauseManager.RestartGame();
    }

    // -------------------------------
    // Stress Test - Rapid Menu Switching
    // -------------------------------

    [ContextMenu("Stress Test - Rapid Menu Switching")]
    public void StressTest_RapidMenus()
    {
        StartCoroutine(RapidMenuCoroutine());
    }

    private IEnumerator RapidMenuCoroutine()
    {
        Debug.Log("[Stress Test] Starting rapid menu switching...");

        for (int i = 0; i < 10; i++)
        {
            menuManager.ShowMainMenu();
            yield return new WaitForSeconds(0.2f);

            settingsManager.OpenSettings();
            yield return new WaitForSeconds(0.2f);

            pauseManager.TriggerPause();
            yield return new WaitForSeconds(0.2f);

            winScreen.ShowWinScreen();
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("[Stress Test] Rapid menu switching completed.");
    }
}
*/