using UnityEngine;

/// <summary>
/// ֱ�߷��еĻ���Ͷ���
/// - Launch(dir, faceRight) ����
/// - ���� targetLayers �ϵĶ���ʱ�����Ե����� Health.Damage(damage)
/// - ���е�����ɱ�ըԤ���壨��ѡ��
/// ���Ҫ��Rigidbody2D + Collider2D��Trigger��
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FireballProjectile : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 8f;          // �����ٶ�
    public float lifeTime = 3f;       // ��������Զ�����

    [Header("Damage")]
    public int damage = 20;           // �˺���ֵ
    public LayerMask targetLayers;    // ֻ������Щ�㣨�� Inspector �� Player �ȣ�

    [Header("VFX (optional)")]
    public GameObject explosionPrefab; // ����ʱ���ɵı�ըԤ���壨��ѡ��

    // �ڲ�״̬
    private Vector2 dir = Vector2.right;
    private Transform ownerRoot;       // �����ߣ����ں����Լ���
    private bool inited;

    void Reset()
    {
        // ȷ����������ײ��������ȷ
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
        // �Զ�����
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// ��ʼ������ѡ�������÷����ߣ����ں����������У���Ŀ��㡣
    /// </summary>
    public void Init(Transform owner, LayerMask mask)
    {
        ownerRoot = owner ? owner.root : null;
        targetLayers = mask;
        inited = true;
    }

    /// <summary>
    /// ���䡣direction �ᱻ��һ����faceRight ���ڷ�ת��ۣ�����Ҫ����
    /// </summary>
    public void Launch(Vector2 direction, bool faceRight)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        // ��ת��ʾ�������ͼ���Ҳ�ͬ��
        var s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * (faceRight ? 1f : -1f), s.y, s.z);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ����������ͬһ���ڵ㣨�����ߣ�
        if (ownerRoot && other.transform.root == ownerRoot) return;

        // �����
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            // ��Ҫʱ��ע�͵���־�Լ�������
            Debug.Log($"[Fireball] Ignore {other.name} (layer {LayerMask.LayerToName(other.gameObject.layer)})");
            return;
        }

        // �˺�������Ѱ�� Health �������ȷ����ı��������� Health.cs��
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

        // ������Ч
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
