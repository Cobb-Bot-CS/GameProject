/*
 * File: FadeHelper.cs
 * Description: (Copied from an online Unity tutorial - used for assignment)
 * Author: TutorialMan (Tutorial Source)
 * Integrated & Modified by: Adam Cobb
 */

using UnityEngine;
using System.Collections;

public class FadeHelper : MonoBehaviour
{
    // This is the reused code from the online tutorial
public IEnumerator FadeCanvas(CanvasGroup cg, float duration)
{
    cg.alpha = 1f;
    cg.gameObject.SetActive(true);
    float elapsed = 0f;
      /*
      while (elapsed < duration)
      {
          elapsed += Time.deltaTime;
          cg.alpha = Mathf.Clamp01(elapsed / duration);
          yield return null;
      }
      */
      yield return null;
    cg.alpha = 1f;
}
}
