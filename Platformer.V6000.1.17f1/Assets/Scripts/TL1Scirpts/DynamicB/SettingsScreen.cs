/*
 * File: SettingsScreen.cs
 * Description: Controls the settings/options UI screen.
 * Author: Adam Cobb
 * Date: 11-16-2025
 */

using UnityEngine;

/// <summary>
/// Handles showing and hiding the Settings screen.
/// </summary>
public class SettingsScreen : UIScreen
{
    /// <summary>
    /// Shows the Settings screen.
    /// </summary>
    public override void Show()
    {
        base.Show();
        Debug.Log("[SettingsScreen] Showing SETTINGS");
    }

    /// <summary>
    /// Hides the Settings screen.
    /// </summary>
    public override void Hide()
    {
        base.Hide();
        Debug.Log("[SettingsScreen] Hiding SETTINGS");
    }
}
