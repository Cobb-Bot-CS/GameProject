/*
 * File: UIScreen.cs
 * Description: Base class for all UI screens, providing virtual methods for show/hide functionality
 *              and a non-virtual method to demonstrate static binding.
 * Author: Adam Cobb
 * Date: 11-16-2025
 */

using UnityEngine;

/// <summary>
/// Represents a generic UI screen. Can be shown or hidden.
/// </summary>
public class UIScreen : MonoBehaviour
{
    /// <summary>
    /// Shows the UI screen. Child classes can override for custom behavior.
    /// </summary>
    public virtual void Show()
    {
        Debug.Log("[UIScreen] Showing a generic UI screen.");
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the UI screen. Child classes can override for custom behavior.
    /// </summary>
    public virtual void Hide()
    {
        Debug.Log("[UIScreen] Hiding a generic UI screen.");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// static binding.
    /// </summary>
    public void StaticExample()
    {
        Debug.Log("[UIScreen] Static binding method called.");
    }
}
