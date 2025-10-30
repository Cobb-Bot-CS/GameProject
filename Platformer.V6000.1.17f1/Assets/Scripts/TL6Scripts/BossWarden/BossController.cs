using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Boss Animator")]
    public Animator anim;
    [Tooltip("The entire breath attack AnimationClip (used to calculate duration for cooldown)")]
    public AnimationClip breathClip;

    [Header("VFX (optional)")]
    [Tooltip("Hitbox to toggle during the attack (e.g., melee fire cone). Can be null.")]
    public GameObject fireCone;
    [Tooltip("Particle effects. Can be null.")]
    public ParticleSystem fireFX;

    [Header("Fireball Projectile")]
    [Tooltip("Fireball spawn point (e.g., in front of the mouth)")]
    public Transform fireMuzzle;
    [Tooltip("Fireball prefab (must have FireballProjectile.cs)")]
    public GameObject fireballPrefab;
    [Tooltip("Which layers the fireball can hit (Must include Player)")]
    public LayerMask fireballTargetLayers = ~0;

    [Header("Control / Test")]
    [Tooltip("Test key: Press K to trigger breath attack")]
    public KeyCode breathKey = KeyCode.K;

    [Header("Facing (optional)")]
    [Tooltip("If specified, Boss will auto-face this Transform")]
    public Transform player;
    public bool autoFacePlayer;

    [Header("Cooldown")]
    [Tooltip("Minimum skill cooldown (seconds). If less than clip length, clip length will be used.")]
    public float breathCooldown = 1.0f;

    // Internal state
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
        // Auto-face player
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

        // Test key trigger breath
        if (Input.GetKeyDown(breathKey))
            TriggerBreath();
    }

    public void TriggerBreath()
    {
        if (busy) return;

        if (busy) return;
        if (!anim || !breathClip) return;
        if (Time.time < nextBreathTime) return;

        busy = true;

        anim.ResetTrigger("Breath");
        anim.SetTrigger("Breath");

        float total = breathClip.length / Mathf.Max(0.0001f, anim.speed);
        nextBreathTime = Time.time + Mathf.Max(total, breathCooldown);
        Invoke(nameof(EndBusy), total);
    }

    void EndBusy() => busy = false;

    public void AnimEvent_StartBreath()
    {
        if (fireCone) fireCone.SetActive(true);
        if (fireFX) fireFX.Play();
    }

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
            proj.Init(transform, fireballTargetLayers);
            proj.Launch(dir, faceRight);
        }
    }

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