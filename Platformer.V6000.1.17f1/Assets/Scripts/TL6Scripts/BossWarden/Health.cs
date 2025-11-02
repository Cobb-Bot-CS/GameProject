using UnityEngine;

/// <summary>
/// 通用生命系统（适用于玩家、敌人、Boss）
/// - currentHP：当前血量
/// - playerMaxHP：最大血量（如果没有外部 Stats）
/// - deathCleanupDelay：死亡延迟销毁时间
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("角色最大生命值（没有外部 Stats 时使用）")]
    public int playerMaxHP = 100;

    [Tooltip("死亡后清理延迟时间")]
    public float deathCleanupDelay = 3f;

    [HideInInspector] public int currentHP;
    private bool isDead = false; // 防止重复死亡

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // 初始化血量
        currentHP = playerMaxHP;
    }

    /// <summary>
    /// 承受伤害
    /// </summary>
    public void Damage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Max(0, currentHP);

        // 播放受伤动画（如果 Animator 有 Hurt 参数）
        if (anim)
            anim.SetTrigger("Hurt");

        if (currentHP <= 0)
            Die();
    }

    /// <summary>
    /// 恢复生命
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, playerMaxHP);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim)
            anim.SetTrigger("Dead");

        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (col)
            col.enabled = false;

        Destroy(gameObject, deathCleanupDelay);
    }
}
