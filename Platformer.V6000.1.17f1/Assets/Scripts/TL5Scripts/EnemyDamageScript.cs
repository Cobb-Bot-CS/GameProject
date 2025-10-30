using UnityEngine;

/*
 * Filename: EnemyDamage.cs
 * Developer: Aj Karki
 * Purpose: Applies damage to the player when the enemy collides.
 */

/// <summary>
/// Handles dealing damage to the player when the enemy touches them.
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    // How much damage the enemy gives the player
    [SerializeField] private int damageAmount = 5;

    // Reference to the player's health script (assign in Inspector)
    [SerializeField] private CharacterHealthScript playerHealth;


    /// <summary>
    /// Called when another collider enters this trigger.
    /// Deals damage to the player if they are hit.
    /// </summary>
    /// <param name="collision">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the enemy touched the player
        if (collision.CompareTag("Player"))
        {
            // If the player has a health script, apply damage
            if (playerHealth != null)
            {
                playerHealth.CharacterHurt(damageAmount);
                Debug.Log("Enemy hit the player! Damage applied: " + damageAmount);
            }
        }
    }
}
