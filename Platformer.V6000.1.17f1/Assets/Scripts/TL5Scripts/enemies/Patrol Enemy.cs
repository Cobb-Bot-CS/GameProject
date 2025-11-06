using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    [SerializeField] private Transform checkPoint;     // will be auto-created if missing
    public float checkDistance = 1f;
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

    // --- Safety nets so fields are never null ---
    void Awake()
    {
        EnsureCheckPoint();
    }
    void OnValidate()
    {
        // Runs in editor when values change; keeps checkpoint present
        if (Application.isPlaying) return;
        EnsureCheckPoint();
    }
    private void EnsureCheckPoint()
    {
        if (checkPoint == null)
        {
            var existing = transform.Find("CheckPoint");
            if (existing != null) checkPoint = existing;
            else
            {
                var go = new GameObject("CheckPoint");
                go.transform.SetParent(transform);
                go.transform.localPosition = checkPointLocal;
                checkPoint = go.transform;
                // No exception anymore even if you forget to assign
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[PatrolEnemy] No GameObject tagged 'Player' found. Chasing/attacking will be skipped.");
    }

    void Update()
    {
        // If no player yet, just patrol without errors
        if (player == null) { Patrol(); return; }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
            ChasePlayer();
        else if (distanceToPlayer <= attackRange)
            TryAttack();
        else
            Patrol();
    }

    void Patrol()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        // Move in the facing direction
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);

        // Raycast ahead to see if there�s ground or a wall
        RaycastHit2D groundInfo = Physics2D.Raycast(checkPoint.position, Vector2.down, checkDistance, groundMask);
        RaycastHit2D wallInfo = Physics2D.Raycast(checkPoint.position, transform.right, checkDistance, groundMask);

        if (!groundInfo.collider || wallInfo.collider)
=======
        // Horizontal move
        rb.velocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.velocity.y);

        // Safe raycasts (only if checkpoint exists)
        if (checkPoint != null)
>>>>>>> Stashed changes
=======
        // Horizontal move
        rb.velocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.velocity.y);

        // Safe raycasts (only if checkpoint exists)
        if (checkPoint != null)
>>>>>>> Stashed changes
=======
        // Horizontal move
        rb.velocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.velocity.y);

        // Safe raycasts (only if checkpoint exists)
        if (checkPoint != null)
>>>>>>> Stashed changes
=======
        // Horizontal move
        rb.velocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.velocity.y);

        // Safe raycasts (only if checkpoint exists)
        if (checkPoint != null)
>>>>>>> Stashed changes
        {
            Vector2 cp = checkPoint.position;
            var groundInfo = Physics2D.Raycast(cp, Vector2.down, checkDistance, groundMask);
            var wallInfo = Physics2D.Raycast(cp, transform.right, checkDistance, groundMask);

            if (!groundInfo.collider || wallInfo.collider)
                Flip();
        }

        if (animator) animator.SetBool("IsChasing", false);
    }

    void ChasePlayer()
    {
        if (animator) animator.SetBool("IsChasing", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0 && !movingRight) Flip();
        else if (direction.x < 0 && movingRight) Flip();
    }

    void TryAttack()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        rb.linearVelocity = Vector2.zero;
=======
        rb.velocity = new Vector2(0f, rb.velocity.y);

>>>>>>> Stashed changes
=======
        rb.velocity = new Vector2(0f, rb.velocity.y);

>>>>>>> Stashed changes
=======
        rb.velocity = new Vector2(0f, rb.velocity.y);

>>>>>>> Stashed changes
=======
        rb.velocity = new Vector2(0f, rb.velocity.y);

>>>>>>> Stashed changes
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator) animator.SetTrigger("Attack");

            // Detect player in a circle around enemy
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
            if (hit != null)
            {
                var health = hit.GetComponent<CharacterHealthScript>();
                if (health != null) health.CharacterHurt(damage);
            }
            lastAttackTime = Time.time; // or Time.time + attackCooldown if you prefer
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        // flip sprite
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;

        // keep checkpoint in front after flip
        if (checkPoint != null)
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

        if (checkPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + Vector3.down * checkDistance);
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + transform.right * checkDistance);
        }
    }
}
