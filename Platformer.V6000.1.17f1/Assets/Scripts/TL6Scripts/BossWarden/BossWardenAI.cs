using System.Collections;
using UnityEngine;

/// <summary>
/// Boss Warden 独立AI：自动检测玩家距离进入/退出战斗
/// </summary>
public class BossWardenAI : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Health health;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireCone;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private DamageHitbox hitbox;

    [Header("=== Stats ===")]
    public float moveSpeed = 3.5f;
    public float attackCooldown = 2f;
    public float detectRange = 20f;   // 玩家进入战斗范围
    public float disengageRange = 25f; // 玩家离开后重置
    public float attackRange = 10f;    // 攻击距离
    public float stunDuration = 1.5f;

    [Header("=== Runtime Debug ===")]
    [SerializeField] private string currentState;

    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private Vector3 spawnPos;

    private enum State
    {
        Idle,
        Chase,
        Fighting,
        Stunned,
        Death
    }
    private State state = State.Idle;

    void Start()
    {
        spawnPos = transform.position;
        if (health == null) health = GetComponent<Health>();
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead || health == null || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        currentState = state.ToString();

        // 自动检测战斗状态
        if (dist < detectRange && state != State.Death)
        {
            if (state == State.Idle) state = State.Chase;
        }
        else if (dist > disengageRange && !isDead)
        {
            state = State.Idle;
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        // 状态逻辑
        switch (state)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Fighting:
                HandleFighting();
                break;
            case State.Stunned:
            case State.Death:
                break;
        }

        // 翻转朝向
        if (!isStunned)
        {
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    #region === 状态处理 ===
    private void HandleIdle()
    {
        animator.SetFloat("Speed", 0);
    }

    private void HandleChase()
    {
        float xDist = Mathf.Abs(player.position.x - transform.position.x);
        animator.SetFloat("Speed", 1);
        rb.linearVelocity = new Vector2(Mathf.Sign(player.position.x - transform.position.x) * moveSpeed, rb.linearVelocity.y);

        if (canAttack && xDist < attackRange)
        {
            state = State.Fighting;
        }
    }

    private void HandleFighting()
    {
        float xDist = Mathf.Abs(player.position.x - transform.position.x);

        if (!isAttacking && canAttack && xDist < attackRange)
        {
            StartCoroutine(PerformFireball());
        }
        else if (xDist >= attackRange)
        {
            state = State.Chase;
        }
    }
    #endregion

    #region === 攻击逻辑 ===
    private IEnumerator PerformFireball()
    {
        if (fireballPrefab == null) yield break;
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger("Breath");
        yield return new WaitForSeconds(0.5f);

        // 生成火球
        GameObject obj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        FireballProjectile proj = obj.GetComponent<FireballProjectile>();

        if (proj != null)
        {
            // ✅ 按Boss当前朝向确定方向
            bool faceRight = transform.localScale.x > 0f;
            Vector2 dir = faceRight ? Vector2.right : Vector2.left;

            proj.Init(transform, LayerMask.GetMask("Player"));
            proj.Launch(dir, faceRight);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }
    #endregion

    #region === 受击/死亡 ===
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        health.Damage((int)dmg);
        animator.SetTrigger("Hurt");

        if (health.currentHP <= 0)
            Die();
        else
            StartCoroutine(StunRecover());
    }

    private IEnumerator StunRecover()
    {
        isStunned = true;
        animator.SetTrigger("Stunned");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        state = State.Chase;
    }

    private void Die()
    {
        isDead = true;
        state = State.Death;
        animator.SetTrigger("Dead");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, 3f);
    }
    #endregion

    #region === 动画事件占位，防止报错 ===
    public void AnimEvent_SpawnFireball() { }
    public void AnimEvent_StartBreath() { }
    public void AnimEvent_StopBreath() { }
    #endregion
}
