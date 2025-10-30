using System.Collections;
using System.Collections.Generic; // 确保引入这个
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("组件")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer spriteRenderersun;

    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 16f;

    [Header("地面检测")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    [Header("攻击设定")]
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private float attackDamage = 50f;

    [Header("格挡设定")] // <<< NEW
    [Tooltip("格挡时免疫伤害的几率 (0.0 到 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float blockChance = 0.75f; // 75% 几率

    // --- 私有变量 ---
    private float horizontalInput;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float attackCooldown = 0.5f; // 简单的攻击冷却，防止连点

    public bool IsBlocking { get; private set; } // <<< NEW: 格挡状态

    void Update()
    {
        // 攻击冷却计时器
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // --- 核心逻辑 ---
        HandleBlockInput();

        if (IsBlocking)
        {
            // 正在格挡时，确保站住不动
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (!isAttacking) // 只有在不格挡且不攻击时，才能移动和跳跃
        {
            HandleMovementAndActions();
        }

        // --- 动画更新 ---
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // --- 输入与动作处理 ---

    private void HandleBlockInput() // <<< NEW
    {
        // 按下右键，并且在地上，并且不在攻击中
        if (Input.GetMouseButtonDown(1) && isGrounded && !isAttacking)
        {
            IsBlocking = true;
        }
        // 松开右键
        else if (Input.GetMouseButtonUp(1))
        {
            IsBlocking = false;
        }
    }

    private void HandleMovementAndActions()
    {
        // 1. 移动输入
        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        Flip();

        // 2. 跳跃输入
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // 3. 攻击输入
        if (Input.GetButtonDown("Fire1") && attackTimer <= 0)
        {
            Attack();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown; // 重置攻击冷却
        animator.SetTrigger("Attack");

        // 使用协程来管理攻击状态和Hitbox，比Invoke更灵活
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine() // <<< MODIFIED: 使用协程替代Invoke
    {
        yield return new WaitForSeconds(0.1f); // 动画前摇

        EnableHitbox();

        yield return new WaitForSeconds(0.25f); // 判定框持续时间

        DisableHitbox();

        isAttacking = false; // 攻击状态结束
    }

    private void EnableHitbox()
    {
        // 确保你的AttackHitbox脚本有这个SetDamage方法
        AttackHitbox hitboxScript = attackHitbox.GetComponent<AttackHitbox>();
        if (hitboxScript != null)
        {
            hitboxScript.SetDamage(attackDamage);
        }
        attackHitbox.SetActive(true);
    }

    private void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }

    private void Flip()
    {
        // 您的翻转逻辑很特别，我将保留它
        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = false;
            spriteRenderersun.flipX = true;
            attackHitbox.transform.localPosition = new Vector3(-Mathf.Abs(attackHitbox.transform.localPosition.x), attackHitbox.transform.localPosition.y, 0);
        }
        else if (horizontalInput > 0)
        {
            spriteRenderer.flipX = true;
            spriteRenderersun.flipX = false;
            attackHitbox.transform.localPosition = new Vector3(Mathf.Abs(attackHitbox.transform.localPosition.x), attackHitbox.transform.localPosition.y, 0);
        }
    }

    private void UpdateAnimator()
    {
       // animator.SetBool("IsBlocking", IsBlocking); // <<< NEW

        if (!IsBlocking)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetBool("IsJumping", !isGrounded);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    public float GetBlockChance() // <<< NEW
    {
        return blockChance;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}