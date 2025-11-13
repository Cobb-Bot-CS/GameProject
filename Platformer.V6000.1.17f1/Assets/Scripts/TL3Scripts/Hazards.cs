/*
 * Filename: Hazards.cs
 * Developer: Alex Johnson
 * Purpose: Enables a hitbox for hazards to detect collisions with player and respawn them
 */

using UnityEngine;

/*
 * Summary: A class that enables a hitbox for hazards to detect collisions with player and respawn them
 * Member Variables:
 *    
 */
public class Hazards : MonoBehaviour
{
   [SerializeField] private float respawnX;
   [SerializeField] private float respawnY;
   [SerializeField] private int hazardDamage;

   private GameObject player;
   private CharacterHealthScript playerHealth;
   private RespawnScript playerRespawn;


   /*
    * Summary: Find the player object and get the health script from it
    */
   private void Awake()
   {
      player = GameObject.Find("Character");
      if (player != null)
      {
         playerHealth = player.GetComponent<CharacterHealthScript>();
         playerRespawn = player.GetComponent<RespawnScript>();
      }
      else
      {
         Debug.LogWarning("Hazard couldn't find player");
      }
   }


   /*
    * Summary: upon colliding with player apply damage and respawn
    * 
    * Parameters:
    *    other - the GameObject that has been collided with
    */
   private void OnTriggerEnter2D(Collider2D other)
   {
      GameObject otherGameObject = other.gameObject;
      // if player boxcollider collided
      if (otherGameObject.CompareTag("Player") && other is BoxCollider2D)
      {
         if (playerHealth != null)
         {
            playerHealth.CharacterHurt(hazardDamage);
         }
         // respawn
         StartCoroutine(playerRespawn.Respawn(respawnX, respawnY));
      }
   }
}
