using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private float damage;

    public void SetDamage(float dmg)
    {
        this.damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure the Boss has the correct Tag or Layer
        if (other.CompareTag("Boss"))
        {
            BossAI_Advanced boss = other.GetComponent<BossAI_Advanced>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("Hit the Boss and dealt " + damage + " damage!");

                // Disable itself to prevent multiple hits from one attack
                gameObject.SetActive(false);
            }
        }
    }
}
