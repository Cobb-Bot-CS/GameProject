using UnityEngine;
using UnityEngine.InputSystem;

public class JewelPuzzleManager : MonoBehaviour
{
   private GameObject pickupPrompt;
   private GameObject interactPrompt;
   private GameObject player;
   private GameObject jewel;
   private GameObject altar1;
   private GameObject altar2;
   private GameObject portal1;
   private GameObject portal2;
   private PuzzleAltar altarPuzzle = new PuzzleAltar();

   private void Awake()
   {
      pickupPrompt = GameObject.Find("PickupPrompt");
      interactPrompt = GameObject.Find("InteractPrompt");
      player = GameObject.Find("Character");
      jewel = GameObject.Find("Jewel");
      altar1 = GameObject.Find("Altar1");
      altar2 = GameObject.Find("Altar2");
      portal1 = GameObject.Find("Portal1");
      portal2 = GameObject.Find("Portal2");
   }

   private void Update()
   {
      if (Mathf.Abs (jewel.transform.position.x - player.transform.position.x) < 4 && Mathf.Abs (jewel.transform.position.y - player.transform.position.y) < 2 && jewel.activeSelf)
      {
         pickupPrompt.transform.position = new Vector3(jewel.transform.position.x, jewel.transform.position.y + 1, 0);
         pickupPrompt.SetActive(true);
      }
      else
      {
         pickupPrompt.SetActive(false);
      }

      if (Mathf.Abs(altar1.transform.position.x - player.transform.position.x) < 4 && Mathf.Abs(altar1.transform.position.y - player.transform.position.y) < 2 && !jewel.activeSelf)
      {
         interactPrompt.transform.position = new Vector3(altar1.transform.position.x, altar1.transform.position.y + 1, 0);
         interactPrompt.SetActive(true);
      }
      else
      {
         interactPrompt.SetActive(false);
      }

      if (Mathf.Abs(altar2.transform.position.x - player.transform.position.x) < 4 && Mathf.Abs(altar2.transform.position.y - player.transform.position.y) < 2 && !jewel.activeSelf)
      {
         interactPrompt.transform.position = new Vector3(altar2.transform.position.x, altar2.transform.position.y + 1, 0);
         interactPrompt.SetActive(true);
      }
      else
      {
         interactPrompt.SetActive(false);
      }
   }


   private void PickupJewel()
   {
      altarPuzzle.AddJewel();
      jewel.SetActive(false);
   }

   private void AltarInteract1()
   {
      if (jewel.activeSelf)
      {
         jewel.SetActive(true);
         portal1.SetActive(true);
         jewel.transform.position = new Vector3(altar1.transform.position.x, altar1.transform.position.y + 1, 0);
         altarPuzzle.SetCondition1(true);
      }
      else
      {
         jewel.SetActive(false);
         portal1.SetActive(false);
         altarPuzzle.SetCondition1(false);
      }
   }

   private void AltarInteract2()
   {
      if (jewel.activeSelf)
      {
         jewel.SetActive(true);
         portal2.SetActive(true);
         jewel.transform.position = new Vector3(altar2.transform.position.x, altar2.transform.position.y + 1, 0);
         altarPuzzle.SetCondition1(true);
      }
      else
      {
         jewel.SetActive(false);
         portal2.SetActive(false);
         altarPuzzle.SetCondition1(false);
      }
   }


   void OnInteract()
   {
      // jewel pickup
      if (pickupPrompt.activeSelf)
      {
         PickupJewel();
      }
      else
      {
         // altar1 interact
         if (interactPrompt.activeSelf && interactPrompt.transform.position.x == altar1.transform.position.x)
         {
            AltarInteract1();
         }
         else
         {
            // altar2 interact
            if (interactPrompt.activeSelf && interactPrompt.transform.position.x == altar2.transform.position.x)
            {
               AltarInteract2();
            }
         }
      }
         gameObject.SetActive(false);
   }
}
