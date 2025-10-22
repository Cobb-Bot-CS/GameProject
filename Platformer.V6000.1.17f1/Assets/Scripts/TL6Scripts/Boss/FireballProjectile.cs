using UnityEngine;

/// <summary>
/// Ö±ï¿½ß·ï¿½ï¿½ÐµÄ»ï¿½ï¿½ï¿½Í¶ï¿½ï¿½ï¿½ï£º
/// - Launch(dir, faceRight) ï¿½ï¿½ï¿½ï¿½
/// - ï¿½ï¿½ï¿½ï¿½ targetLayers ï¿½ÏµÄ¶ï¿½ï¿½ï¿½Ê±ï¿½ï¿½ï¿½ï¿½ï¿½Ôµï¿½ï¿½ï¿½ï¿½ï¿½ Health.Damage(damage)
/// - ï¿½ï¿½ï¿½Ðµï¿½ï¿½ï¿½ï¿½ï¿½É±ï¿½Õ¨Ô¤ï¿½ï¿½ï¿½å£¨ï¿½ï¿½Ñ¡ï¿½ï¿½
/// ï¿½ï¿½ï¿½Òªï¿½ï¿½Rigidbody2D + Collider2Dï¿½ï¿½Triggerï¿½ï¿½
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;          // ï¿½ï¿½ï¿½ï¿½ï¿½Ù¶ï¿½
    public float lifeTime = 3f;       // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ô¶ï¿½ï¿½ï¿½ï¿½ï¿½

    [Header("Damage")]
    public int damage = 20;           // ï¿½Ëºï¿½ï¿½ï¿½Öµ
    public LayerMask targetLayers;    // Ö»ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ð©ï¿½ã£¨ï¿½ï¿½ Inspector ï¿½ï¿½ Player ï¿½È£ï¿½

    [Header("VFX (optional)")]
    public GameObject explosionPrefab; // ï¿½ï¿½ï¿½ï¿½Ê±ï¿½ï¿½ï¿½ÉµÄ±ï¿½Õ¨Ô¤ï¿½ï¿½ï¿½å£¨ï¿½ï¿½Ñ¡ï¿½ï¿½

    // ï¿½Ú²ï¿½×´Ì¬
    private Vector2 dir = Vector2.right;
<<<<<<< Updated upstream
    private Transform ownerRoot;       // ï¿½ï¿½ï¿½ï¿½ï¿½ß£ï¿½ï¿½ï¿½ï¿½Úºï¿½ï¿½ï¿½ï¿½Ô¼ï¿½ï¿½ï¿½
    private bool inited;
=======
    private Transform ownerRoot;       // ·¢ÉäÕß£¨ÓÃÓÚºöÂÔ×Ô¼º£©
    // private bool inited; // <-- ÒÑÉ¾³ý£ºÕâ¸ö±äÁ¿Ã»ÓÐ±»Ê¹ÓÃ
>>>>>>> Stashed changes

    void Reset()
    {
        // È·ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½×²ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È·
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
        // ï¿½Ô¶ï¿½ï¿½ï¿½ï¿½ï¿½
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// ï¿½ï¿½Ê¼ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ñ¡ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ã·ï¿½ï¿½ï¿½ï¿½ß£ï¿½ï¿½ï¿½ï¿½Úºï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ð£ï¿½ï¿½ï¿½Ä¿ï¿½ï¿½ã¡£
    /// </summary>
    public void Init(Transform owner, LayerMask mask)
    {
        ownerRoot = owner ? owner.root : null;
        targetLayers = mask;
        // inited = true; // <-- ÒÑÉ¾³ý£ºÕâ¸ö±äÁ¿Ã»ÓÐ±»Ê¹ÓÃ
    }

    /// <summary>
    /// ï¿½ï¿½ï¿½ä¡£direction ï¿½á±»ï¿½ï¿½Ò»ï¿½ï¿½ï¿½ï¿½faceRight ï¿½ï¿½ï¿½Ú·ï¿½×ªï¿½ï¿½Û£ï¿½ï¿½ï¿½ï¿½ï¿½Òªï¿½ï¿½ï¿½ï¿½
    /// </summary>
    public void Launch(Vector2 direction, bool faceRight)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        // ï¿½ï¿½×ªï¿½ï¿½Ê¾ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Í¼ï¿½ï¿½ï¿½Ò²ï¿½Í¬ï¿½ï¿½
        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1f : -1f), s.y, s.z);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Í¬Ò»ï¿½ï¿½ï¿½Úµã£¨ï¿½ï¿½ï¿½ï¿½ï¿½ß£ï¿½
        if (ownerRoot && other.transform.root == ownerRoot) return;

        // ï¿½ï¿½ï¿½ï¿½ï¿½
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            // ï¿½ï¿½ÒªÊ±ï¿½ï¿½×¢ï¿½Íµï¿½ï¿½ï¿½Ö¾ï¿½Ô¼ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
            Debug.Log($"[Fireball] Ignore {other.name} (layer {LayerMask.LayerToName(other.gameObject.layer)})");
            return;
        }

        // ï¿½Ëºï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ñ°ï¿½ï¿½ Health ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½È·ï¿½ï¿½ï¿½ï¿½Ä±ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ Health.csï¿½ï¿½
        var hp = other.GetComponent<CharacterHealthScript>();
        if (hp != null)
        {
            hp.CharacterHurt(damage);
            Debug.Log($"[Fireball] Hit {other.name} for {damage}");
        }
        else
        {
            Debug.Log($"[Fireball] {other.name} has no Health component");
        }

        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ð§
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}