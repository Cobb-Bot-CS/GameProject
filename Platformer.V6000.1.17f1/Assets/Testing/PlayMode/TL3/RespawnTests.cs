using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RespawnTests : MonoBehaviour
{
   private GameObject player;
   private RespawnScript respawner;
   private GameObject fadePanelObject;
   private CanvasGroup fadePanel;


   [OneTimeSetUp]
   public void Setup()
   {
      SceneManager.LoadScene("AlexTestScene");
   }

   [UnityTest]
   public IEnumerator FadeTest()
   {
      // get the player
      player = GameObject.Find("Character");
      // get script
      if (player != null)
      {
         respawner = player.GetComponent<RespawnScript>();
      }
      else
      {
         Debug.LogWarning("RespawnTest could not find player");
      }
      // get fadePanel
      fadePanelObject = GameObject.Find("Black");
      if (fadePanelObject != null)
      {
         fadePanel = fadePanelObject.GetComponent<CanvasGroup>();
      }
      else
      {
         Debug.LogWarning("RespawnTest could not find ScreenFade");
      }

      // valid fades
      Assert.AreEqual(0, fadePanel.alpha);
      // fade
      yield return respawner.Fade(0, 1);
      // check alpha
      Assert.AreEqual(1, fadePanel.alpha);

      yield return respawner.Fade(1, 0); ;
      Assert.AreEqual(0, fadePanel.alpha);

      yield return respawner.Fade(0, 0); ;
      Assert.AreEqual(0, fadePanel.alpha);

      // invalid fades
      yield return respawner.Fade(0, 2);
      Assert.AreEqual(1, fadePanel.alpha);

      yield return respawner.Fade(2, 0);
      Assert.AreEqual(0, fadePanel.alpha);

      yield return respawner.Fade(0, -1);
      Assert.AreEqual(0, fadePanel.alpha);

      yield return respawner.Fade(-1, 1);
      Assert.AreEqual(1, fadePanel.alpha);
   }


   [UnityTest]
   public IEnumerator RespawnTest()
   {
      // get the player
      player = GameObject.Find("Character");
      // get script
      if (player != null)
      {
         respawner = player.GetComponent<RespawnScript>();
      }
      else
      {
         Debug.LogWarning("RespawnTest could not find player");
      }
      // get fadePanel
      fadePanelObject = GameObject.Find("Black");
      if (fadePanelObject != null)
      {
         fadePanel = fadePanelObject.GetComponent<CanvasGroup>();
      }
      else
      {
         Debug.LogWarning("RespawnTest could not find ScreenFade");
      }

      // respawn player
      yield return respawner.Respawn(5, 5);
      // check location
      Assert.AreEqual(5, player.transform.position.x, player.transform.position.y);
   }
}
