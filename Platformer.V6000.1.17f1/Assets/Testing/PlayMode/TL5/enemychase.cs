using UnityEngine;

public class EnemyDamageOnTouch : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldown = 1f;
    private float lastHitTime = -10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < lastHitTime + damageCooldown) return;

        if (other.CompareTag("Player"))
        {
            CharacterHealth playerHealth = other.GetComponent<CharacterHealth>();
            if (playerHealth != null)
            {
                playerHealth.CharacterHurt(Mathf.Max(0, damage));
                Debug.Log("Enemy damaged player! Current HP: " + playerHealth.GetHealth());
                lastHitTime = Time.time;
            }
        }
    }
}
