using UnityEngine;
using System.Collections;   // 保留这一行

[RequireComponent(typeof(Collider2D))]
public class DamageHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 20;
    public LayerMask targetLayers;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    // ✅ BossWardenAI 兼容接口
    public void EnableHitbox()
    {
        if (col != null)
        {
            col.enabled = true;
            Debug.Log("[Hitbox] Enabled");
        }
    }

    public void DisableHitbox()
    {
        if (col != null)
        {
            col.enabled = false;
            Debug.Log("[Hitbox] Disabled");
        }
    }

    // 你原本的定时启用逻辑可以保留
    public void EnableFor(float seconds) => StartCoroutine(DoEnable(seconds));

    private IEnumerator DoEnable(float t)
    {
        EnableHitbox();
        yield return new WaitForSeconds(t);
        DisableHitbox();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 层过滤
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        // 造成伤害
        var hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.Damage(damage);
            Debug.Log($"[Hitbox] {other.name} took {damage} damage");
        }
    }
}
