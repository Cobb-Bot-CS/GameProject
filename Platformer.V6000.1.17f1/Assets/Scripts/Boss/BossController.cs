using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Boss 身上的 Animator（状态机 AC_Boss 挂在这）")]
    public Animator anim;
    [Tooltip("喷火整套动画的 AnimationClip（用于计算总时长做冷却）")]
    public AnimationClip breathClip;

    [Header("VFX (optional)")]
    [Tooltip("喷火前/中/后若需要开关的命中体（近战火焰扇形等，可为空）")]
    public GameObject fireCone;
    [Tooltip("粒子特效（可为空）")]
    public ParticleSystem fireFX;

    [Header("Fireball Projectile")]
    [Tooltip("火球的出生点（放在嘴前）")]
    public Transform fireMuzzle;
    [Tooltip("火球预制体（挂有 FireballProjectile.cs）")]
    public GameObject fireballPrefab;
    [Tooltip("火球命中哪些层（务必勾 Player）")]
    public LayerMask fireballTargetLayers = ~0;

    [Header("Control / Test")]
    [Tooltip("快捷键：按 K 触发喷火")]
    public KeyCode breathKey = KeyCode.K;

    [Header("Facing (optional)")]
    [Tooltip("若指定，Boss 会自动朝向玩家")]
    public Transform player;
    public bool autoFacePlayer = false;

    [Header("Cooldown")]
    [Tooltip("喷火技能的最小冷却（秒）。若小于动画时长，会使用动画时长）")]
    public float breathCooldown = 1.0f;

    // 内部状态
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
        // 自动面向玩家
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

        // 测试键触发喷火
        if (Input.GetKeyDown(breathKey))
            TriggerBreath();
    }

    /// <summary>对外接口：请求一次喷火。</summary>
    public void TriggerBreath()
    {
        if (!anim || !breathClip) return;
        if (Time.time < nextBreathTime) return; // 冷却中

        busy = true;

        anim.ResetTrigger("Breath");
        anim.SetTrigger("Breath");              // 状态机里 AnyState → Boss_Breath 使用 Trigger: Breath

        // 冷却 = max(动画时长, 设定冷却)
        float total = breathClip.length / Mathf.Max(0.0001f, anim.speed);
        nextBreathTime = Time.time + Mathf.Max(total, breathCooldown);
        Invoke(nameof(EndBusy), total);
    }

    void EndBusy() => busy = false;

    // ========= 被动画事件调用 =========
    // 在 Boss_Breath 动画里关键帧添加事件：AnimEvent_StartBreath / AnimEvent_SpawnFireball / AnimEvent_StopBreath

    /// <summary>前摇结束，开始喷火（打开近战火焰/粒子）</summary>
    public void AnimEvent_StartBreath()
    {
        if (fireCone) fireCone.SetActive(true);
        if (fireFX) fireFX.Play();
    }

    /// <summary>出招帧：生成火球（远程弹）</summary>
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
            // 把发射者与命中层传给投射物，避免命中自己 & 正确过滤层
            proj.Init(transform, fireballTargetLayers);
            proj.Launch(dir, faceRight);
        }
    }

    /// <summary>收招：关闭火焰/粒子</summary>
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
