using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    [SerializeField] private Transform checkPoint;
    public float checkDistance = 1f;   // FIXED: 10f → 1f (normal ground check)
    public LayerMask groundMask;
    public LayerMask playerMask;

    [Header("Attack Settings")]
    public float chaseRange = 5f;
    public float attackRange = 1.2f;
    public int damage = 10;
    public float attackCooldown = 1f;

    [Header("Auto CheckPoint Offset (local)")]
    [SerializeField] private Vector2 checkPointLocal = new Vector2(0.6f, -0.1f);

    private bool movingRight = true;
    private float lastAttackTime;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    // ⭐ NEW: Prevent fast flipping
    private float lastFlipTime = 0f;
    private float flipCooldown = 0.3f;

    void Awake() => EnsureCheckPoint();

    void OnValidate()
    {
        if (Application.isPlaying) return;
        EnsureCheckPoint();
    }

    private void EnsureCheckPoint()
    {
        if (checkPoint != null) return;

        var existing = transform.Find("CheckPoint");
        if (existing != null)
        {
            checkPoint = existing;
            return;
        }

        var go = new GameObject("CheckPoint");
        go.transform.SetParent(transform);
        go.transform.localPosition = checkPointLocal;
        checkPoint = go.transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[PatrolEnemy] No GameObject tagged 'Player' found.");
    }

    void Update()
    {
        if (player == null) { Patrol(); return; }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
            ChasePlayer();
        else if (distanceToPlayer <= attackRange)
            TryAttack();
        else
            Patrol();
    }

    // ---------- FIXED PATROL ----------
    void Patrol()
    {
        rb.linearVelocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.linearVelocity.y);

        Vector2 origin = checkPoint ? (Vector2)checkPoint.position : (Vector2)transform.position;
        RaycastHit2D groundInfo = Physics2D.Raycast(origin, Vector2.down, checkDistance, groundMask);
        RaycastHit2D wallInfo = Physics2D.Raycast(origin, transform.right, checkDistance, groundMask);

        // ⭐ FIXED: Flip only if cooldown passed (no more shaking)
        if (!groundInfo.collider || wallInfo.collider)
        {
            if (Time.time - lastFlipTime > flipCooldown)
            {
                Flip();
                lastFlipTime = Time.time;
            }
        }

        if (animator) animator.SetBool("IsChasing", false);
    }

    void ChasePlayer()
    {
        if (animator) animator.SetBool("IsChasing", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0f && !movingRight) Flip();
        else if (direction.x < 0f && movingRight) Flip();
    }

    void TryAttack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (Time.time - lastAttackTime < attackCooldown) return;

        if (animator) animator.SetTrigger("Attack");

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        if (hit != null)
        {
            var health = hit.GetComponent<CharacterHealthScript>();
            if (health != null) health.CharacterHurt(damage);
        }

        lastAttackTime = Time.time;
    }

    void Flip()
    {
        movingRight = !movingRight;

        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;

        if (checkPoint)
        {
            var lp = checkPoint.localPosition;
            lp.x *= -1f;
            checkPoint.localPosition = lp;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (checkPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + Vector3.down * checkDistance);
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + transform.right * checkDistance);
        }
    }
}
