using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("BC Mode Settings")]
    public bool BCMode = false;             // True or false toggle
    public Text bcModeStatusText;           // Reference to on-screen text

    void Start()
    {
        UpdateBCModeDisplay();
    }

    public void ToggleBCMode(bool isOn)
    {
        BCMode = isOn;
        UpdateBCModeDisplay();
    }

    private void UpdateBCModeDisplay()
    {
        if (bcModeStatusText != null)
        {
            bcModeStatusText.gameObject.SetActive(BCMode);
            bcModeStatusText.text = "BC MODE ACTIVE";
        }
    }
}
