using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Canvases")]
    public GameObject mainMenuUI;
    public GameObject settingsMenuUI;

    void Start()
    {
        ShowMainMenu();
    }

    // Show Main Menu and hide Settings
    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }

    // Show Settings Menu and hide Main Menu
    public void ShowSettingsMenu()
    {
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
}
