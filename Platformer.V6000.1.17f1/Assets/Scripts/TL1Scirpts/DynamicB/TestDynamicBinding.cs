using UnityEngine;

public class TestDynamicBinding : MonoBehaviour
{
    public UIScreen pauseMenuScreen;
    public UIScreen settingsScreen;
    private UIScreen currentScreen;

    private void Start()
    {
        currentScreen = pauseMenuScreen;
        currentScreen.Show();

        currentScreen = settingsScreen;
        currentScreen.Show();
    }
}
