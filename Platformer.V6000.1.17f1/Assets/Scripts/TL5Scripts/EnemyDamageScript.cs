using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5; // how much damage enemy gives
    [SerializeField] private CharacterHealthScript playerHealth;
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
