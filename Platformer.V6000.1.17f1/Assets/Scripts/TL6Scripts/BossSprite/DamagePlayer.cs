using UnityEngine;
/*
 * Filename: DamagePlayer.cs
 * Developer: Qiwei Liang
 * Purpose: This file is to damage player
 */

public class DamagePlayer : MonoBehaviour
{
    private float damage;
    private GameObject hitVFX; // 用于存储命中特效的预制体
    public bool destroyOnHit = true;

    // --- NEW: 使用 Setup 方法来接收来自Boss的数据 ---
    public void Setup(float dmg, GameObject vfx)
    {
        //BossAI 调用：damageScript.Setup(currentAttack.damage, currentAttack.hitVFX);

        this.damage = dmg;
        this.hitVFX = vfx;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterHealth playerHealth = other.GetComponent<CharacterHealth>();
            if (playerHealth != null)
            {
                playerHealth.CharacterHurt((int)damage);

                // --- NEW: 生成命中特效 (如果被指定了) ---
                if (hitVFX != null)
                {
                    Instantiate(hitVFX, other.transform.position, Quaternion.identity);
                }
            }

            if (destroyOnHit)
            {
                //延迟0.5s删除
                Destroy(gameObject,0.5f);
            }
        }
    }
}