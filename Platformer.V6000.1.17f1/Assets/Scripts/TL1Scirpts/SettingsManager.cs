/*
 * File: SettingsManager.cs
 * Description: Manages player settings, including volume control and navigation.
 * Author: Adam Cobb
 * Date: 11-05-2025
 */

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Header("Menu Settings")]
    public GameObject settingsMenuUI; // Reference to Settings Menu Canvas

    private void Start()
    {
    }

    /// <summary>
    /// Opens the settings menu.
    /// </summary>
    public void OpenSettings()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f; // Optional: freeze time if used in-game
    }

    /// <summary>
    /// Closes the settings menu and resumes game or returns to main menu.
    /// </summary>
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
