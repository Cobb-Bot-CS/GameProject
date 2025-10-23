using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;          // Flight speed
    public float lifeTime = 3f;       // Auto-destroy after this many seconds

    [Header("Damage")]
    public int damage = 20;           // Damage value
    public LayerMask targetLayers;    // Only hit these layers (set in Inspector)

    [Header("VFX (optional)")]
    public GameObject explosionPrefab; // Explosion prefab on hit (optional)

    // Internal state
    private Vector2 dir = Vector2.right;
<<<<<<< Updated upstream:Platformer.V6000.1.17f1/Assets/Scripts/TL6Scripts/FireballProjectile.cs
    private Transform ownerRoot;       // Owner transform (to ignore self)
    // private bool inited; 
=======
    private Transform ownerRoot;       // �����ߣ����ں����Լ���
    // private bool inited; // <-- ��ɾ�����������û�б�ʹ��
>>>>>>> Stashed changes:Platformer.V6000.1.17f1/Assets/Scripts/TL6Scripts/Boss/FireballProjectile.cs

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
<<<<<<< Updated upstream:Platformer.V6000.1.17f1/Assets/Scripts/TL6Scripts/FireballProjectile.cs
=======
        // inited = true; // <-- ��ɾ�����������û�б�ʹ��
>>>>>>> Stashed changes:Platformer.V6000.1.17f1/Assets/Scripts/TL6Scripts/Boss/FireballProjectile.cs
    }

    public void Launch(Vector2 direction, bool faceRight)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        // Flip visual based on direction
        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1f : -1f), s.y, s.z);
    }

    void Update()
    {
        // This line moves the fireball
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
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

        // --- �˺��߼��޸Ŀ�ʼ ---
        // �����ҵ� Health �ű� (������ CharacterHealthScript)
        var hp = other.GetComponent<Health>();
        if (hp != null)
        {
            // ����ͳһ�� Damage ����
            hp.Damage(damage);
            Debug.Log($"[Fireball] Hit {other.name} for {damage}");
        }
        else
        {
            Debug.Log($"[Fireball] {other.name} has no Health component");
        }
        // --- �˺��߼��޸Ľ��� ---

        // Hit VFX
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}