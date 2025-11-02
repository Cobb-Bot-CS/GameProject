using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private float damage;
    private GameObject hitVFX; // 用于存储命中特效的预制体
    public bool destroyOnHit = true;

    // --- NEW: 使用 Setup 方法来接收来自Boss的数据 ---
    public void Setup(float dmg, GameObject vfx)
    {
        this.damage = dmg;
        this.hitVFX = vfx;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                // --- NEW: 生成命中特效 (如果被指定了) ---
                if (hitVFX != null)
                {
                    Instantiate(hitVFX, other.transform.position, Quaternion.identity);
                }
            }

            if (destroyOnHit)
            {
                Destroy(gameObject,0.5f);
            }
        }
    }
}