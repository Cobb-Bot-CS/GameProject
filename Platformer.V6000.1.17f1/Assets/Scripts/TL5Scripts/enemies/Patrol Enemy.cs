using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float checkDistance = 1f;
    public LayerMask groundMask;
    public LayerMask playerMask;

    [Header("Attack Settings")]
    public float chaseRange = 5f;
    public float attackRange = 1.2f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private bool movingRight = true;
    private float lastAttackTime;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            TryAttack();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Move in the facing direction
        rb.velocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.velocity.y);

        // Raycast ahead to see if there’s ground or a wall
        RaycastHit2D groundInfo = Physics2D.Raycast(checkPoint.position, Vector2.down, checkDistance, groundMask);
        RaycastHit2D wallInfo = Physics2D.Raycast(checkPoint.position, transform.right, checkDistance, groundMask);

        if (!groundInfo.collider || wallInfo.collider)
        {
            Flip();
        }

        if (animator != null)
            animator.SetBool("IsChasing", false);
    }

    void ChasePlayer()
    {
        if (animator != null)
            animator.SetBool("IsChasing", true);

        // Move toward player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Flip toward player
        if (direction.x > 0 && !movingRight)
            Flip();
        else if (direction.x < 0 && movingRight)
            Flip();
    }

    void TryAttack()
    {
        rb.velocity = Vector2.zero;
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
            if (hit != null)
            {
                CharacterHealthScript health = hit.GetComponent<CharacterHealthScript>();
                if (health != null)
                    health.CharacterHurt(damage);
            }
            lastAttackTime = Time.time;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
