using UnityEngine;

/// <summary>
/// 直线飞行的火球投射物：
/// - Launch(dir, faceRight) 启动
/// - 命中 targetLayers 上的对象时，尝试调用其 Health.Damage(damage)
/// - 命中点可生成爆炸预制体（可选）
/// 组件要求：Rigidbody2D + Collider2D（Trigger）
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;          // 飞行速度
    public float lifeTime = 3f;       // 多少秒后自动销毁

    [Header("Damage")]
    public int damage = 20;           // 伤害数值
    public LayerMask targetLayers;    // 只命中这些层（在 Inspector 勾 Player 等）

    [Header("VFX (optional)")]
    public GameObject explosionPrefab; // 命中时生成的爆炸预制体（可选）

    // 内部状态
    private Vector2 dir = Vector2.right;
    private Transform ownerRoot;       // 发射者（用于忽略自己）
    private bool inited;

    void Reset()
    {
        // 确保刚体与碰撞器配置正确
        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }

        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnEnable()
    {
        // 自动销毁
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 初始化（可选）：设置发射者（用于忽略自身命中）与目标层。
    /// </summary>
    public void Init(Transform owner, LayerMask mask)
    {
        ownerRoot = owner ? owner.root : null;
        targetLayers = mask;
        inited = true;
    }

    /// <summary>
    /// 发射。direction 会被归一化；faceRight 用于翻转外观（如需要）。
    /// </summary>
    public void Launch(Vector2 direction, bool faceRight)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        // 翻转显示（如果贴图左右不同）
        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1f : -1f), s.y, s.z);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 忽略自身或同一根节点（发射者）
        if (ownerRoot && other.transform.root == ownerRoot) return;

        // 层过滤
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            // 需要时可注释掉日志以减少噪音
            Debug.Log($"[Fireball] Ignore {other.name} (layer {LayerMask.LayerToName(other.gameObject.layer)})");
            return;
        }

        // 伤害：尝试寻找 Health 组件（请确保你的被击对象有 Health.cs）
        var hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.Damage(damage);
            Debug.Log($"[Fireball] Hit {other.name} for {damage}");
        }
        else
        {
            Debug.Log($"[Fireball] {other.name} has no Health component");
        }

        // 命中特效
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
