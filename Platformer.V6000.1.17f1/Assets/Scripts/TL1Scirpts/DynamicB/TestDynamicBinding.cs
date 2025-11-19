/*
 * File: TestDynamicBinding.cs
 * Description: Demonstrates dynamic binding (Show/Hide) and static binding (StaticExample).
 * Author: Adam Cobb
 * Date: 11-16-2025
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestDynamicBinding : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private UIScreen pauseMenuScreen;
    [SerializeField] private UIScreen settingsScreen;

    [Header("Optional UI Feedback")]
    [SerializeField] private Text statusText;

    private UIScreen currentScreen;

    private void Start()
    {
        if (pauseMenuScreen != null && settingsScreen != null)
        {
            // Dynamic binding example
            currentScreen = pauseMenuScreen;
            currentScreen.Show();    // Calls PauseMenuScreen.Show() → dynamic
            settingsScreen.Hide();

            // Static binding example
            currentScreen.StaticExample(); // Calls UIScreen.StaticExample() → static

            UpdateStatusText();
        }
        else
        {
            Debug.LogError("[TestDynamicBinding] UIScreen references not assigned in Inspector.");
        }
    }

  private void Update()
{
    if (Input.GetKeyDown(KeyCode.LeftControl))
    {
        Debug.Log("[TestDynamicBinding] Left Control detected");
        if (currentScreen != null)
        {
            currentScreen.Hide();
            currentScreen = (currentScreen == pauseMenuScreen) ? settingsScreen : pauseMenuScreen;
            currentScreen.Show();
            currentScreen.StaticExample();
            UpdateStatusText();
        }
    }
}

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = currentScreen == pauseMenuScreen ? "Pause Menu Active" : "Settings Active";
        }
    }
}
