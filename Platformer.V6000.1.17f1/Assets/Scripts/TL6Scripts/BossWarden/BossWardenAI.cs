using System.Collections;
using UnityEngine;

/// <summary>
/// Boss Warden 主 AI 控制脚本
/// 整合阶段逻辑、远程火息、火球发射、近战飞踢、受击眩晕与死亡判定。
/// 增加 ArenaController 检测逻辑接口（ActivateBattle / DeactivateBattle）
/// </summary>
public class BossWardenAI : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Health health;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireCone; // 含 FireBreathDamage 的物体
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private DamageHitbox hitbox; // 飞踢或撞击范围

    [Header("=== Stats ===")]
    public float moveSpeed = 3.5f;
    public float retreatSpeed = 4f;
    public float attackCooldown = 2f;
    public float phase2Threshold = 0.7f;
    public float stunDuration = 1.5f; // 眩晕时间
    public float flyKickForce = 15f;
    public float fireballDamage = 20f;

    [Header("=== Runtime Debug ===")]
    [SerializeField] private string currentState;
    [SerializeField] private int currentPhase = 1;

    private bool isDead = false;
    private bool isStunned = false;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isBattleStarted = false; // ✅ 新增：战斗状态
    private Vector3 spawnPos;

    private enum State
    {
        Dormant,
        Idle,
        Chase,
        Retreat,
        Fighting,
        Stunned,
        Death
    }
    private State state = State.Dormant;

    void Start()
    {
        spawnPos = transform.position;
        state = State.Dormant; // ✅ 一开始不动
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead || health == null) return;
        if (!isBattleStarted) return; // ✅ 没进入战斗区时不执行AI逻辑

        float hpPercent = (float)health.currentHP / Mathf.Max(1, health.playerMaxHP);
        currentPhase = (hpPercent <= phase2Threshold) ? 2 : 1;
        currentState = state.ToString();

        switch (state)
        {
            case State.Idle:
                HandleIdle();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Retreat:
                HandleRetreat();
                break;
            case State.Fighting:
                HandleFighting();
                break;
            case State.Stunned:
            case State.Death:
                break;
        }

        // 朝向翻转
        if (player != null && !isStunned)
        {
            if (player.position.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    #region === State Logic ===
    private void HandleIdle()
    {
        animator.SetFloat("Speed", 0);
        if (Vector2.Distance(transform.position, player.position) < 15f)
            state = State.Chase;
    }

    private void HandleChase()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("Speed", 1);
        rb.linearVelocity = new Vector2(Mathf.Sign(player.position.x - transform.position.x) * moveSpeed, rb.linearVelocity.y);

        if (canAttack && dist <= 10f)
            StartCoroutine(PerformFireball());
    }

    private void HandleRetreat()
    {
        if (player == null) return;
        animator.SetFloat("Speed", 1);
        Vector2 dir = (transform.position.x < player.position.x) ? Vector2.left : Vector2.right;
        rb.linearVelocity = dir * retreatSpeed;

        if (canAttack)
            StartCoroutine(PerformFireBreath());
    }

    private void HandleFighting()
    {
        if (player == null) return;
        if (!isAttacking && canAttack)
        {
            if (currentPhase == 1)
                StartCoroutine(PerformFireball());
            else
                StartCoroutine(PerformFlyKick());
        }
    }
    #endregion

    #region === Attacks ===
    private IEnumerator PerformFireball()
    {
        if (fireballPrefab == null) yield break;
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger("Breath");
        yield return new WaitForSeconds(0.5f);

        GameObject obj = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D projRb = obj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            float dir = Mathf.Sign(player.position.x - transform.position.x);
            projRb.linearVelocity = new Vector2(dir * 8f, 0f);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }

    private IEnumerator PerformFireBreath()
    {
        if (fireCone == null) yield break;
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger("Breath");
        yield return new WaitForSeconds(0.4f);
        fireCone.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fireCone.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }

    private IEnumerator PerformFlyKick()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetTrigger("FlyKick");

        yield return new WaitForSeconds(0.6f);
        rb.AddForce(new Vector2(Mathf.Sign(player.position.x - transform.position.x) * flyKickForce, 5f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(attackCooldown + 1f);
        isAttacking = false;
        canAttack = true;
    }
    #endregion

    #region === Reactions ===
    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        health.Damage((int)dmg);
        animator.SetTrigger("Hurt");

        if (health.currentHP <= 0)
            Die();
        else if (currentPhase == 1)
            StartCoroutine(Phase1HitReaction());
        else
            StartCoroutine(Phase2Stun());
    }

    private IEnumerator Phase1HitReaction()
    {
        state = State.Retreat;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PerformFireBreath());
    }

    private IEnumerator Phase2Stun()
    {
        isStunned = true;
        animator.SetTrigger("Stunned");
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        state = State.Fighting;
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

    #region === Animator Events ===
    public void AnimEvent_StartBreath() { if (fireCone != null) fireCone.SetActive(true); }
    public void AnimEvent_StopBreath() { if (fireCone != null) fireCone.SetActive(false); }
    public void AnimEvent_SpawnFireball() { if (fireballPrefab != null && firePoint != null) Instantiate(fireballPrefab, firePoint.position, Quaternion.identity); }
    #endregion

    #region === Arena Controller Integration ===
    // ✅ 当玩家进入Boss区域时由ArenaController调用
    public void ActivateBattle()
    {
        Debug.Log("[BossWardenAI] Boss battle activated!");
        isBattleStarted = true;
        state = State.Idle;
        canAttack = true;
        rb.simulated = true;
    }

    // ✅ 当玩家离开Boss区域时由ArenaController调用
    public void DeactivateBattle()
    {
        Debug.Log("[BossWardenAI] Boss battle deactivated!");
        isBattleStarted = false;
        state = State.Dormant;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPos; // ✅ 回到初始点
    }
    #endregion
}
