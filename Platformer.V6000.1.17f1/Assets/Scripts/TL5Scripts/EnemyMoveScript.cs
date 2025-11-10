using UnityEngine;

/*
 * Filename: Playerawarness.cs
 * Developer: Aj Karki
 * Purpose: Detects the player and applies damage when in range.
 */

public class Playerawarness : MonoBehaviour
{
    [Header("Detection Settings")]
    private Transform player;                  // Player Transform
    private float detectRadius = 6f;           // How far the enemy can see the player
    private float attackRadius = 1.5f;         // How close the enemy must be to attack
   // private float moveSpeed = 2f;              // Enemy movement speed
    private float attackCooldown = 1f;         // Delay between attacks
    private LayerMask playerLayer;             // Assign to Player layer in Inspector

    private MovespeedSuper moveSpeedObj = new Movespeed();

    private float nextAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Auto-find the player if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Detect & Chase
        if (distance <= detectRadius && distance > attackRadius)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Attack
        if (distance <= attackRadius && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeedObj.speed(), rb.linearVelocity.y);

        // Flip facing direction
        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();
    }

    private void AttackPlayer()
    {
        if (animator != null)
            animator.SetTrigger("attack");

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, playerLayer);
        if (hit != null)
        {
            CharacterHealthScript playerHealth = hit.GetComponent<CharacterHealthScript>();
            if (playerHealth != null)
            {
                // ? FIXED: call CharacterHurt instead of CharacterTakeDamage
                playerHealth.CharacterHurt(5);
                Debug.Log("Enemy attacked the player!");
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

public class MovespeedSuper
{

   public virtual float speed()
  // public float speed()
    {
        return 100f;
    }
}

public class Movespeed: MovespeedSuper
{

   public override  float speed()
  // public float speed()
    {
        return 2f;
    }
}

