using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("����ѡ������ Boss/Enemy ���������ñ�")]
    public BossStats stats;

    [Tooltip("����� Boss/Enemy����������������Ϻ����ٶ��������ʱ��")]
    public float deathCleanupDelay = 3f;

    [Tooltip("����� Player��������ҵ��������ֵ������ Stats �ֶ�Ϊ��ʱ��Ч��")]
    public int playerMaxHP = 100;

    [HideInInspector]
    public int currentHP;

    // �������
    private Animator anim;
    private BossController bossController; // �� Boss ��
    private Rigidbody2D rb;
    private Collider2D col;

    private bool isDead = false; // ��ֹ�ظ�����

    public void Awake()
    {
        // �ؼ����� BossStats ��ʼ�� HP (�����)
        if (stats != null)
        {
            currentHP = stats.maxHP; //
        }
        else
        {
            // ������Ϊ�� Player ��������λ��ʹ�� playerMaxHP
            currentHP = playerMaxHP;
        }

        // �ؼ�����ȡ�������������
        anim = GetComponent<Animator>();
        bossController = GetComponent<BossController>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Damage(int amount)
    {
        // ����Ѿ����ˣ���Ҫ�ظ���Ѫ
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - amount);
       

        if (currentHP == 0)
        {
            isDead = true; // ���Ϊ����
            OnDeath();
        }
    }

    void OnDeath()
    {
        Debug.Log($"[Health] {name} died");

        // 1. ������������
        if (anim)
        {
            anim.SetTrigger("Dead"); // ʹ�� "Dead" ��ƥ����� Animator
        }

        // 2. ����� Boss������ Boss �߼�
        if (bossController)
        {
            bossController.enabled = false;

            // ֹͣ���������˶�����ײ (������ API)
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            if (col)
            {
                col.enabled = false;
            }

            // ���������˺��ж������� FireBreathDamage��
            var hitboxes = GetComponentsInChildren<Collider2D>();
            foreach (var hitbox in hitboxes)
            {
                if (hitbox.isTrigger)
                {
                    hitbox.enabled = false;
                }
            }

            // 5. �ڶ���������Ϻ����� GameObject
            Destroy(gameObject, deathCleanupDelay);
        }
        else
        {
            // ����� Player��ִ����ҵ������߼�
            // (���磺����������������ʾ "Game Over" �����)
            var sr = GetComponent<SpriteRenderer>();
            if (sr) sr.color = Color.gray; // ��ʱ�������������
            Debug.Log($"[Health] Player {name} died");
            // ע�⣺ͨ�����ǲ������� Destroy(player)
        }
    }

    // --- ��ʱ���Դ��� ---
    // (��ɺ�ǵ�ɾ��)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("[TEST] L ������, ������������...");
            Damage(currentHP); // ��ɵ��ڵ�ǰѪ�����˺�
        }
    }
    // --- ��ʱ������� ---
}