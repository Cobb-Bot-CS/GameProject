using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;          // Flight speed
    public float lifeTime = 3f;       // Auto-destroy after this many seconds
    public Vector2 direction = Vector2.right; // ✅ 新增字段，让BossWardenAI能控制方向

    [Header("Damage")]
    public int damage = 20;           // Damage value
    public LayerMask targetLayers;    // Only hit these layers (set in Inspector)

    [Header("VFX (optional)")]
    public GameObject explosionPrefab; // Explosion prefab on hit (optional)

    // Internal state
    private Transform ownerRoot;       // Owner transform (to ignore self)

    void Reset()
    {
        // Ensure Rigidbody and Collider are configured correctly
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
        // Auto-destroy
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    public void Init(Transform owner, LayerMask mask)
    {
        ownerRoot = owner ? owner.root : null;
        targetLayers = mask;
    }

    public void Launch(Vector2 dir, bool faceRight)
    {
        direction = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;

        // Flip visual based on direction
        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1f : -1f), s.y, s.z);
    }

    void Update()
    {
        // ✅ BossWardenAI.cs 调用 direction 后，这里会自动移动
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore self or owner
        if (ownerRoot && other.transform.root == ownerRoot) return;

        // Layer filter
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            Debug.Log($"[Fireball] Ignore {other.name} (layer {LayerMask.LayerToName(other.gameObject.layer)})");
            return;
        }

        // Damage logic
        var hp = other.GetComponent<Health>();
        if (hp != null)
        {
            hp.Damage(damage);
            Debug.Log($"[Fireball] Hit {other.name} for {damage}");
        }

        // Hit VFX
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
