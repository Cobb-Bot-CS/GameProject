using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Boss ���ϵ� Animator��״̬�� AC_Boss �����⣩")]
    public Animator anim;
    [Tooltip("������׶����� AnimationClip�����ڼ�����ʱ������ȴ��")]
    public AnimationClip breathClip;

    [Header("VFX (optional)")]
    [Tooltip("���ǰ/��/������Ҫ���ص������壨��ս�������εȣ���Ϊ�գ�")]
    public GameObject fireCone;
    [Tooltip("������Ч����Ϊ�գ�")]
    public ParticleSystem fireFX;

    [Header("Fireball Projectile")]
    [Tooltip("����ĳ����㣨������ǰ��")]
    public Transform fireMuzzle;
    [Tooltip("����Ԥ���壨���� FireballProjectile.cs��")]
    public GameObject fireballPrefab;
    [Tooltip("����������Щ�㣨��ع� Player��")]
    public LayerMask fireballTargetLayers = ~0;

    [Header("Control / Test")]
    [Tooltip("��ݼ����� K �������")]
    public KeyCode breathKey = KeyCode.K;

    [Header("Facing (optional)")]
    [Tooltip("��ָ����Boss ���Զ��������")]
    public Transform player;
    public bool autoFacePlayer;

    [Header("Cooldown")]
    [Tooltip("����ܵ���С��ȴ���룩����С�ڶ���ʱ������ʹ�ö���ʱ����")]
    public float breathCooldown = 1.0f;

    // �ڲ�״̬
    bool busy = false;
    float nextBreathTime = 0f;
    Rigidbody2D rb;

    void Awake()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // �Զ��������
        if (autoFacePlayer && player)
        {
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.02f)
            {
                var s = transform.localScale;
                s.x = Mathf.Sign(dx) * Mathf.Abs(s.x);
                transform.localScale = s;
            }
        }

        // ���Լ��������
        if (Input.GetKeyDown(breathKey))
            TriggerBreath();
    }

    /// <summary>����ӿڣ�����һ�����</summary>
    public void TriggerBreath()
    {
        if (!anim || !breathClip) return;
        if (Time.time < nextBreathTime) return; // ��ȴ��

        busy = true;

        anim.ResetTrigger("Breath");
        anim.SetTrigger("Breath");              // ״̬���� AnyState �� Boss_Breath ʹ�� Trigger: Breath

        // ��ȴ = max(����ʱ��, �趨��ȴ)
        float total = breathClip.length / Mathf.Max(0.0001f, anim.speed);
        nextBreathTime = Time.time + Mathf.Max(total, breathCooldown);
        Invoke(nameof(EndBusy), total);
    }

    void EndBusy() => busy = false;

    // ========= �������¼����� =========
    // �� Boss_Breath ������ؼ�֡�����¼���AnimEvent_StartBreath / AnimEvent_SpawnFireball / AnimEvent_StopBreath

    /// <summary>ǰҡ��������ʼ��𣨴򿪽�ս����/���ӣ�</summary>
    public void AnimEvent_StartBreath()
    {
        if (fireCone) fireCone.SetActive(true);
        if (fireFX) fireFX.Play();
    }

    /// <summary>����֡�����ɻ���Զ�̵���</summary>
    public void AnimEvent_SpawnFireball()
    {
        if (!fireballPrefab) return;

        Vector3 spawnPos = fireMuzzle ? fireMuzzle.position : transform.position;
        bool faceRight = transform.lossyScale.x >= 0f;
        Vector2 dir = faceRight ? Vector2.right : Vector2.left;

        var go = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        var proj = go.GetComponent<FireballProjectile>();
        if (proj)
        {
            // �ѷ����������в㴫��Ͷ������������Լ� & ��ȷ���˲�
            proj.Init(transform, fireballTargetLayers);
            proj.Launch(dir, faceRight);
        }
    }

    /// <summary>���У��رջ���/����</summary>
    public void AnimEvent_StopBreath()
    {
        if (fireCone) fireCone.SetActive(false);
        if (fireFX) fireFX.Stop();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
    }
#endif
}
