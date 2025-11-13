/*
 * File: SettingsManager.cs
 * Description: Manages player settings, including volume control and navigation.
 * Author: Adam Cobb
 * Date: 11-05-2025
 */

using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles the game settings such as audio volume via buttons and menu navigation.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [Header("Menu Settings")]
    public GameObject settingsMenuUI;   // Reference to Settings Menu Canvas

    private void Start()
    {
        
    }
    /// <summary>
    /// Opens the settings menu.
    /// </summary>
    public void OpenSettings()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f; // Optional: freeze game while settings are open
    }

    /// <summary>
    /// Closes the settings menu and resumes the game.
    /// </summary>
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
