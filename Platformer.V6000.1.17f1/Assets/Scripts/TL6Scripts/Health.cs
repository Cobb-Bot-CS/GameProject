using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("（可选）关联 Boss/Enemy 的属性配置表")]
    public BossStats stats;

    [Tooltip("（如果 Boss/Enemy）死亡动画播放完毕后，销毁对象所需的时间")]
    public float deathCleanupDelay = 3f;

    [Tooltip("（如果 Player）设置玩家的最大生命值（仅当 Stats 字段为空时生效）")]
    public int playerMaxHP = 100;

    [HideInInspector]
    public int currentHP;

    // 组件引用
    private Animator anim;
    private BossController bossController; // 仅 Boss 有
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isDead = false; // 防止重复死亡

    public void Awake()
    {
        // 关键：从 BossStats 初始化 HP (如果有)
        if (stats != null)
        {
            currentHP = stats.maxHP; //
        }
        else
        {
            // 否则，认为是 Player 或其他单位，使用 playerMaxHP
            currentHP = playerMaxHP;
        }

        // 关键：获取其他组件的引用
        anim = GetComponent<Animator>();
        bossController = GetComponent<BossController>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Damage(int amount)
    {
        // 如果已经死了，不要重复扣血
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - amount);
       

        if (currentHP == 0)
        {
            isDead = true; // 标记为死亡
            OnDeath();
        }
    }

    void OnDeath()
    {
        Debug.Log($"[Health] {name} died");

        // 1. 触发死亡动画
        if (anim)
        {
            anim.SetTrigger("Dead"); // 使用 "Dead" 来匹配你的 Animator
        }

        // 2. 如果是 Boss，禁用 Boss 逻辑
        if (bossController)
        {
            bossController.enabled = false;

            // 停止所有物理运动和碰撞 (已修正 API)
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            if (col)
            {
                col.enabled = false;
            }

            // 禁用所有伤害判定框（例如 FireBreathDamage）
            var hitboxes = GetComponentsInChildren<Collider2D>();
            foreach (var hitbox in hitboxes)
            {
                if (hitbox.isTrigger)
                {
                    hitbox.enabled = false;
                }
            }

            // 5. 在动画播放完毕后销毁 GameObject
            Destroy(gameObject, deathCleanupDelay);
        }
        else
        {
            // 如果是 Player，执行玩家的死亡逻辑
            // (例如：播放死亡动画，显示 "Game Over" 界面等)
            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.color = Color.gray; // 临时的玩家死亡表现
            Debug.Log($"[Health] Player {name} died");
            // 注意：通常我们不在这里 Destroy(player)
        }
    }

    // --- 临时测试代码 ---
    // (完成后记得删除)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("[TEST] L 键按下, 触发测试死亡...");
            Damage(currentHP); // 造成等于当前血量的伤害
        }
    }
    // --- 临时代码结束 ---
}