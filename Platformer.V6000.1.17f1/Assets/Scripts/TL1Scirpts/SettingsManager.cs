/*
 * File: SettingsManager.cs
 * Description: Manages player settings, including volume control and navigation.
 * Author: Adam Cobb
 * Date: 11-05-2025
 */

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Handles the game settings such as audio volume and navigation between menus.
/// </summary>
/// <remarks>
/// Member Variables:
///     audioMixer - reference to the AudioMixer controlling master volume.
///     volumeSlider - UI slider to adjust volume.
///     settingsMenuUI - reference to the Settings Menu Canvas.
/// </remarks>
public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;     // Reference to main audio mixer
    public Slider volumeSlider;       // UI slider for controlling volume

    [Header("Menu Settings")]
    public GameObject settingsMenuUI; // Reference to Settings Menu Canvas

    private void Start()
    {
        // Load saved volume preference or default to 0 dB
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        audioMixer.SetFloat("Volume", savedVolume);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
    }

    /// <summary>
    /// Sets the master audio volume based on slider input.
    /// </summary>
    /// <param name="volume">Slider value between -80 (mute) and 0 (max).</param>
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume); // Save preference
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
