using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;
    public float lifeTime = 3f;

    [Header("Damage")]
    public int damage = 20;
    public LayerMask targetLayers;
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private Transform ownerRoot;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnEnable()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    public void Init(Transform owner, LayerMask mask)
    {
        ownerRoot = owner ? owner.root : null;
        targetLayers = mask;
    }

    public void Launch(Vector2 dir, bool faceRight)
    {
        Vector2 moveDir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.right;
        rb.linearVelocity = moveDir * speed;

        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1 : -1), s.y, s.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (ownerRoot && other.transform.root == ownerRoot) return;
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        var hp = other.GetComponent<Health>();
        if (hp != null) hp.Damage(damage);

        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
