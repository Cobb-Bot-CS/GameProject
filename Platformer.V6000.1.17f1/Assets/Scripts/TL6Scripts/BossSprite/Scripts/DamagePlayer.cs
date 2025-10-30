using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private float damage;
    private GameObject hitVFX; // ���ڴ洢������Ч��Ԥ����
    public bool destroyOnHit = true;

    // --- NEW: ʹ�� Setup ��������������Boss������ ---
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

                // --- NEW: ����������Ч (�����ָ����) ---
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