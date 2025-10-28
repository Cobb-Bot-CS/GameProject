/*
 * File: SettingsManager.cs
 * Description: Manages and displays player settings including BC Mode toggle.
 * Author: Adam Cobb
 * Date: 10-27-2025
 */

using UnityEngine;
using UnityEngine.UI;


/// 
/// Handles game settings such as toggling BC Mode.
/// 
/// Member Variables:
///    BCMode - whether BC Mode is active.
///    bcModeStatusText - UI text displaying BC Mode status.
/// 
public class SettingsManager : MonoBehaviour
{
   [Header("BC Mode Settings")]
   public bool BCMode = false;             // True or false toggle
   public Text bcModeStatusText;           // Reference to on-screen text


   /// 
   /// Called when the scene starts. Updates BC Mode display.
   /// 
   void Start()
   {
      UpdateBCModeDisplay();
   }


   /// 
   /// Toggles BC Mode on or off and updates the display.
   /// 
   /// <param name="isOn">True to enable BC Mode, false to disable.</param>
   public void ToggleBCMode(bool isOn)
   {
      BCMode = isOn;
      UpdateBCModeDisplay();
   }


   /// 
   /// Updates the on-screen text to reflect current BC Mode status.
   /// 
   private void UpdateBCModeDisplay()
   {
      if (bcModeStatusText != null)
      {
         bcModeStatusText.gameObject.SetActive(BCMode);
         bcModeStatusText.text = "BC MODE ACTIVE";
      }
   }
}
