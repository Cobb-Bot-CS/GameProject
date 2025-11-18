using UnityEngine;

/*
 * filename: SoundTriggers.cs
 * Developer: Urvashi Gupta
 * Purpose: Provides centralized methods for triggering various game sounds through AudioManager
 */

/*
 * Summary: Utility class that exposes sound trigger methods for use in animation events, UI events, and game logic
 *
 * Member Variables:
 * None - This class contains only static sound trigger methods
 */
public class SoundTriggers : MonoBehaviour
{
   //==================== PLAYER SOUNDS ====================//

   /*
    * Summary: Plays player jump sound effect
    */
   public void PlayerJump()
   {
      AudioManager.Instance.Play("Jump");
   }


   /*
    * Summary: Plays player hurt sound effect
    */
   public void PlayerHurt()
   {
      AudioManager.Instance.Play("PlayerHurt");
   }



   /*
    * Summary: Plays player death sound effect
    */
   public void PlayerDeath()
   {
      AudioManager.Instance.PlayOneShot("PlayerDeath");
   }



   //==================== GAMEPLAY SOUNDS ====================//


   /*
    * Summary: Plays weapon pickup sound effect
    */
   public void WeaponPickup()
   {
      AudioManager.Instance.Play("WeaponPickup");
   }



   /*
    * Summary: Plays sword attack sound effect
    */
   public void SwordAttack()
   {
      AudioManager.Instance.Play("SwordAttack");
   }



   //==================== ENEMY SOUNDS ====================//

   /*
    * Summary: Plays enemy death sound effect
    */
   public void EnemyDie()
   {
      AudioManager.Instance.Play("EnemyDie");
   }



   /*
    * Summary: Plays enemy chase/alert sound effect
    */
   public void EnemyChase()
   {
      AudioManager.Instance.Play("EnemyChase");
   }



   //==================== ITEM SOUNDS ====================//

   /*
    * Summary: Plays coin collection sound effect
    */
   public void CoinPickup()
   {
      AudioManager.Instance.Play("CoinPickup");
   }



   //==================== UI SOUNDS ====================//

   /*
    * Summary: Plays UI button click sound effect (one-shot)
    */
   public void UIButtonClick()
   {
      AudioManager.Instance.PlayOneShot("ButtonClick");
   }



   /*
    * Summary: Plays pause menu open sound effect
    */
   public void PauseOpen()
   {
      AudioManager.Instance.Play("PauseOpen");
   }



   /*
    * Summary: Plays pause menu close sound effect
    */
   public void PauseClose()
   {
      AudioManager.Instance.Play("PauseClose");
   }
}