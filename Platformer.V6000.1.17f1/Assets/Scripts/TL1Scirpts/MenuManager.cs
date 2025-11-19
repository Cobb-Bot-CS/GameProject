using UnityEditor.SearchService;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Canvases")]
    public GameObject mainMenuUI;
    public GameObject settingsMenuUI;

   private AudioManager audioManager;

   void Start()
    {
      audioManager = FindFirstObjectByType<AudioManager>();
      ShowMainMenu();
    }

    // Show Main Menu and hide Settings
    public void ShowMainMenu()
    {
        audioManager.Play("ButtonClick");
        Time.timeScale = 0f;
        mainMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }

    // Show Settings Menu and hide Main Menu
    public void ShowSettingsMenu()
    {
        audioManager.Play("ButtonClick");
        mainMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
    
    // Overloaded functions -> Static Binding
    public void OpenMenu()
    {
        Debug.Log("Opening default main menu");
    }

    public void OpenMenu(string menuName)
    {
        Debug.Log("Opening menu: " + menuName);
    }

    public void OpenMenu(int index)
    {
        Debug.Log("Opening menu by index: " + index);
    }
    
}
