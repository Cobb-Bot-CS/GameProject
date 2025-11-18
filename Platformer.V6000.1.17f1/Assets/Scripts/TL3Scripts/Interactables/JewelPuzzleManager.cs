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
      portal1.SetActive(false);
      portal2.SetActive(false);
   }

   private void Update()
   {
      if (Mathf.Abs (jewel.transform.position.x - player.transform.position.x) < 2 && Mathf.Abs (jewel.transform.position.y - player.transform.position.y) < 2 && jewel.activeSelf)
      {
         pickupPrompt.transform.position = new Vector3(jewel.transform.position.x, jewel.transform.position.y + 1, 0);
         pickupPrompt.SetActive(true);
      }
      else
      {
         pickupPrompt.SetActive(false);
      }

      bool near1 = Mathf.Abs(altar1.transform.position.x - player.transform.position.x) < 2 && Mathf.Abs(altar1.transform.position.y - player.transform.position.y) < 2;
      bool near2 = Mathf.Abs(altar2.transform.position.x - player.transform.position.x) < 2 && Mathf.Abs(altar2.transform.position.y - player.transform.position.y) < 2;

      if ((near1 || near2)  && !jewel.activeSelf)
      {
         interactPrompt.SetActive(true);
         if (near1)
         {
            interactPrompt.transform.position = new Vector3(altar1.transform.position.x, altar1.transform.position.y + 2, 0);
         }
         else
         {
            interactPrompt.transform.position = new Vector3(altar2.transform.position.x, altar2.transform.position.y + 2, 0);
         }
      }
      else
      {
         interactPrompt.SetActive(false);
      }
   }

   private void PickupJewel()
   {
      altarPuzzle.SetCondition1(false);
      jewel.SetActive(false);
      
      // Play coin pickup sound through Audio Manager
      AudioManager audioManager = FindFirstObjectByType<AudioManager>();
      if (audioManager != null)
      {
         audioManager.Play("CoinPickup");
      }
   }

   private void AltarInteract1()
   {
      if (!jewel.activeSelf)
      {
         jewel.SetActive(true);
         portal1.SetActive(true);
         jewel.transform.position = new Vector3(altar1.transform.position.x, altar1.transform.position.y + 1.5f, 0);
         altarPuzzle.SetCondition1(true);
         
         // Play sound for placing jewel - using WeaponPickup sound
         AudioManager audioManager = FindFirstObjectByType<AudioManager>();
         if (audioManager != null)
         {
            audioManager.Play("WeaponPickup");
         }
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
      if (!jewel.activeSelf)
      {
         jewel.SetActive(true);
         portal2.SetActive(true);
         jewel.transform.position = new Vector3(altar2.transform.position.x, altar2.transform.position.y + 1.5f, 0);
         altarPuzzle.SetCondition1(true);
         
         // Play sound for placing jewel - using WeaponPickup sound
         AudioManager audioManager = FindFirstObjectByType<AudioManager>();
         if (audioManager != null)
         {
            audioManager.Play("WeaponPickup");
         }
      }
      else
      {
         jewel.SetActive(false);
         portal2.SetActive(false);
         altarPuzzle.SetCondition1(false);
      }
   }

   private void OnInteract(InputValue Button)
   {
      // jewel pickup
      if (pickupPrompt.activeSelf && !altarPuzzle.PickedUp1())
      {
         PickupJewel();
      }
      else
      {
         // altar1 interact
         if ((interactPrompt.activeSelf || pickupPrompt.activeSelf) && interactPrompt.transform.position.x == altar1.transform.position.x && altarPuzzle.PickedUp1())
         {
            AltarInteract1();
         }
         else
         {
            // altar2 interact
            if ((interactPrompt.activeSelf || pickupPrompt.activeSelf) && interactPrompt.transform.position.x == altar2.transform.position.x && altarPuzzle.PickedUp1())
            {
               AltarInteract2();
            }
         }
      }
   }
}