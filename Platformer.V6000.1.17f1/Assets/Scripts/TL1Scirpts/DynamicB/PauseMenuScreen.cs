/*
 * File: PauseMenuScreen.cs
 * Description: Derived UI screen with fade animation
 * Author: Adam Cobb
 */

using UnityEngine;

public class PauseMenuScreen : UIScreen
{
    [SerializeField] private CanvasGroup canvasGroup;
    private FadeHelper fader;

    private void Awake()
    {
      canvasGroup.alpha = 1f;
      fader = gameObject.AddComponent<FadeHelper>();
      canvasGroup.alpha = 1f;
   }

    public override void Show()
    {
        base.Show();  // static binding :)
        canvasGroup.alpha = 1f;
        StartCoroutine(fader.FadeCanvas(canvasGroup, 1f));
      canvasGroup.alpha = 1f;
   }

    public override void Hide()
    {
        base.Hide();  // static binding :)
        canvasGroup.alpha = 1f;
        gameObject.SetActive(false);
    }
}
