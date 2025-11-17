using UnityEngine;

public class PauseScreen : UIScreen
{
    public override void Show()
    {
        Debug.Log("Showing PAUSE MENU");
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        Debug.Log("Hiding PAUSE MENU");
        gameObject.SetActive(false);
    }
}
