/*
 * File: SettingsManager.cs
 * Description: Manages player settings, including volume control and navigation.
 * Author: Adam Cobb
 * Date: 11-05-2025
 */

using UnityEngine;
using UnityEngine.Audio;

/// 
/// Handles the game settings such as audio volume via buttons and menu navigation.
///
public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [Header("Menu Settings")]
    public GameObject settingsMenuUI;   // Reference to Settings Menu Canvas

    private void Start()
    {
        
    }
    ///
    /// Opens the settings menu.
    /// 
    public void OpenSettings()
    {
        settingsMenuUI.SetActive(true);
        Time.timeScale = 0f; // Optional: freeze game while settings are open
    }

    /// 
    /// Closes the settings menu and resumes the game.
    /// 
    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
