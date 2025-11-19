/*
 * Filename: Respawn.cs
 * Developer: Alex Johnson
 * Purpose: respawns the player at the specified location
 */

using System;
using System.Collections;
using UnityEngine;

/*
 * Summary: A class that respawns the player at a specified location
 * Member Variables:
 *    
 */

public class RespawnScript : MonoBehaviour
{
   [SerializeField] float fadeTime = 1.5f;
   [SerializeField] CanvasGroup fadePanel;
   private GameObject LevelManagerObject;
   private LevelManager levelManager;


   public IEnumerator Respawn(float xLocation, float yLocation)
   {
      if (fadePanel != null)
      {
         // pause everything but this script
         Time.timeScale = 0f;
         // fade black screen in
         StartCoroutine(Fade(0, 1));
         // wait for fade to complete, use realtime to ignore timescale
         yield return new WaitForSecondsRealtime(fadeTime);
         // set player position
         transform.position = new Vector3(xLocation, yLocation, 0f);
         // fade black screen out
         StartCoroutine(Fade(1, 0));
         yield return new WaitForSecondsRealtime(fadeTime);
         Time.timeScale = 1f;
      }
      else
      {
         Debug.LogWarning("Respawner could not find canvas");
         transform.position = new Vector3(xLocation, yLocation, 0f);
      }
   }


   public IEnumerator Fade(float startAlpha, float endAlpha)
   {
      float timer = 0f;
      while (timer < fadeTime)
      {
         // use unscaled time to ignore timescale
         timer += Time.unscaledDeltaTime;
         fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeTime);
         yield return null;
      }
      fadePanel.alpha = endAlpha;
   }


   private void Awake()
   {
      LevelManagerObject = GameObject.Find("LevelManager");
      if (LevelManagerObject != null)
      {
         levelManager = LevelManagerObject.GetComponent<LevelManager>();
      }
      else
      {
         Debug.LogWarning("Respawn script could not find Level Manager");
      }
      if (levelManager != null) {
         transform.position = levelManager.GetSpawnPos();
      }
      else
      {
         Debug.LogWarning("Respawn script could not find Level Manager Script");
      }
   }
}
