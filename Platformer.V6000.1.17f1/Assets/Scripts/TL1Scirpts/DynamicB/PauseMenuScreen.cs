/*
 * File: PauseMenuScreen.cs
 * Description: Controls the pause menu UI screen.
 * Author: Adam Cobb
 * Date: 11-16-2025
 */

using UnityEngine;

/// <summary>
/// Handles showing and hiding the Pause Menu.
/// </summary>
public class PauseMenuScreen : UIScreen
{
    public override void Show()
    {
        base.Show();
        Debug.Log("[PauseMenuScreen] Showing PAUSE MENU");
    }

    public override void Hide()
    {
        base.Hide();
        Debug.Log("[PauseMenuScreen] Hiding PAUSE MENU");
    }

    /// <summary>
    /// Same method name as base class, but this is NOT virtual.
    /// This will demonstrate static binding when called through a UIScreen reference.
    /// </summary>
    public new void StaticExample()
    {
        Debug.Log("[PauseMenuScreen] StaticExample in child called (but static binding uses base).");
    }
}
