using UnityEngine;

public class SettingsScreen : UIScreen
{
    public override void Show()
    {
        Debug.Log("Showing SETTINGS");
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        Debug.Log("Hiding SETTINGS");
        gameObject.SetActive(false);
    }
}
