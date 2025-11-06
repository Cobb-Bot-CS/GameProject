using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SpawnLocation { Self, OnPlayer } // Define two spawn locations: self or on the player

[System.Serializable]
public class BossAttack
{
    public string attackName;
    public float animationIndex;
    public GameObject attackPrefab; // The prefab of the skill (for example, a fireball)

    [Header("Effects")]
    [Tooltip("VFX played when the boss starts casting (optional)")]
    public GameObject castVFX;
    [Tooltip("VFX played when the skill hits the player (optional)")]
    public GameObject hitVFX;

    [Header("Properties")]
    public SpawnLocation spawnLocation;
    public float minRange = 0f;
    public float maxRange = 5f;
    public float damage = 10f; // Damage for melee attacks
    public float cooldown = 2f;
}

public class BossAI_Advanced : MonoBehaviour
{
    [Header("UI & Debugging")]
    [SerializeField] private TextMeshProUGUI statusText; // Text component for showing current state
    [Tooltip("Projectile spawn point such as fireball position")]
    [SerializeField] private Transform firePoint;

    // Boss state machine
    private enum State { Dormant, Returning, Idle, Bored, Fighting, Death }
    private State currentState;

    [Header("Core References")]
    [SerializeField] private Transform player;
    [SerializeField] private BossArenaController arenaController;
    private Vector3 initialPosition;

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CharacterHealthScript playerHealth;

    [Header("Basic Stats")]
    public Slider healthBar;
    public float maxHealth = 2000f;
    private float currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float dashSpeed = 4f;

    [Header("Battle Phases")]
    private int currentPhase = 1;
    [SerializeField] private float phase2Threshold = 0.7f;
    [SerializeField] private float phase3Threshold = 0.4f;

    [Header("Attack Library (Configure in Inspector)")]
    [SerializeField] private BossAttack[] meleeAttacks;
    [SerializeField] private BossAttack[] rangedPhase1Attacks;
    [SerializeField] private BossAttack[] rangedPhase2Attacks;
    [SerializeField] private BossAttack[] rangedPhase3Attacks;
    [SerializeField] private BossAttack pushbackAttack;

    [Header("AI Behavior Settings")]
    [SerializeField] private float meleeRange = 3f;
    [SerializeField] private float boredTimer = 3f;
    [SerializeField] private float jumpInterval = 8f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float actionCooldown = 1.5f;

    private float timeSinceLastAction = 0f;
    private float timeSinceLostPlayer = 0f;
    private float timeSinceLastJump = 0f;
    private BossAttack currentAttack;
    private bool isFlipped = false;
    [Tooltip("Visual child object that should be flipped")]
    [SerializeField] private Transform visualsTransform;

    void Start()
    {
        initialPosition = transform.position;
        currentState = State.Idle;
        UpdateStatusText("Idle");
        //animator.Play("Idle");

        currentHealth = maxHealth;
       
       
    }

    void Update()
    {
        if (currentState == State.Death) return;

        // State machine logic
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Bored:
                BoredState();
                break;
            case State.Returning:
                ReturningState();
                break;
            case State.Fighting:
                FightingState();
                break;
        }
    }

    #region --- State Logic ---

    private void IdleState()
    {
        //animator.Play("Idle");
        timeSinceLostPlayer += Time.deltaTime;
        if (timeSinceLostPlayer > boredTimer)
        {
            currentState = State.Bored;
            UpdateStatusText("Bored");
        }
    }

    private void BoredState()
    {
        animator.Play("Bored");
    }

    private void ReturningState()
    {
        animator.Play("Move");
        UpdateStatusText("Returning");
        transform.position = Vector2.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
        LookAtPosition(initialPosition);

        if (Vector2.Distance(transform.position, initialPosition) < 0.1f)
        {
            currentState = State.Idle;
            timeSinceLostPlayer = 0f;
            UpdateStatusText("Idle");
        }
    }

    private void FightingState()
    {
        if (player == null) { DisengageTarget(); return; }

        LookAtPlayer();
        timeSinceLastAction += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;

        if (timeSinceLastJump > jumpInterval)
        {
            Jump();
        }

        if (timeSinceLastAction > actionCooldown)
        {
            UpdateStatusText("Thinking...");
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Debug.Log($"--- AI Decision --- Distance to player: {distanceToPlayer}");

            // Decision 1: Melee attack
            if (distanceToPlayer <= meleeRange)
            {
                Debug.Log($"Decision: Player in melee range (<= {meleeRange}). Execute melee attack.");
                PerformAttack(GetRandomAttack(meleeAttacks));
                return;
            }

            // Decision 2: Ranged attack
            BossAttack rangedAttack = GetAvailableRangedAttack(distanceToPlayer);
            if (rangedAttack != null)
            {
                Debug.Log($"Decision: Found ranged skill '{rangedAttack.attackName}'. Execute ranged attack.");
                PerformAttack(rangedAttack);
                return;
            }

            // Decision 3: Move closer
            UpdateStatusText("Moving to attack");
            Debug.Log($"Decision: Player out of range, moving closer.");
            rb.linearVelocity = new Vector2((player.position.x > transform.position.x ? 1 : -1) * moveSpeed, rb.linearVelocity.y);
        }

        if (timeSinceLastAction <= actionCooldown && currentState == State.Fighting && rb.linearVelocity.magnitude > 0.1f)
        {
            UpdateStatusText("Moving");
        }

        if (timeSinceLastAction <= actionCooldown)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
        }
    }
    #endregion

    #region --- Combat Execution ---

    private void PerformAttack(BossAttack attack)
    {
        if (attack == null) return;
        currentAttack = attack;
        UpdateStatusText(attack.attackName);

        if (attack.castVFX != null)
        {
            Instantiate(attack.castVFX, transform.position, Quaternion.identity);
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("AttackIndex", attack.animationIndex);
        animator.SetTrigger("Attack");
        timeSinceLastAction = 0f;
    }

    public void SpawnAttackPrefab()
    {
        if (currentAttack == null)
        {
            Debug.LogError("SpawnAttackPrefab was called, but currentAttack is null!");
            return;
        }

        if (currentAttack.attackPrefab == null)
        {
            Debug.Log($"Performing melee damage check: {currentAttack.attackName}...");
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentAttack.maxRange, LayerMask.GetMask("Player"));
            foreach (Collider2D hit in hits)
            {
                CharacterHealthScript playerHealth = hit.GetComponent<CharacterHealthScript>();
                if (playerHealth != null)
                {
                    playerHealth.CharacterHurt((int)currentAttack.damage);
                    if (currentAttack.hitVFX != null)
                    {
                        Instantiate(currentAttack.hitVFX, hit.transform.position, Quaternion.identity);
                    }
                    break;
                }
            }
        }
        else
        {
            GameObject projectile = null;

            switch (currentAttack.spawnLocation)
            {
                case SpawnLocation.Self:
                    if (firePoint == null)
                    {
                        Debug.LogError("Skill set to 'Self' but no FirePoint specified! Using boss center instead.");
                        projectile = Instantiate(currentAttack.attackPrefab, transform.position, Quaternion.identity);
                    }
                    else
                    {
                        projectile = Instantiate(currentAttack.attackPrefab, firePoint.position, firePoint.rotation);
                    }
                    Debug.Log($"Spawned projectile from fire point: {currentAttack.attackPrefab.name}");
                    break;

                case SpawnLocation.OnPlayer:
                    if (player != null)
                    {
                        projectile = Instantiate(currentAttack.attackPrefab, player.position, Quaternion.identity);
                        Debug.Log($"Spawned projectile on player: {currentAttack.attackPrefab.name}");
                    }
                    break;
            }

            if (projectile != null)
            {
                DamagePlayer damageScript = projectile.GetComponent<DamagePlayer>();
                if (damageScript != null)
                {
                    damageScript.Setup(currentAttack.damage, currentAttack.hitVFX);
                }

                ProjectileMover mover = projectile.GetComponent<ProjectileMover>();
                if (mover != null && player != null)
                {
                    mover.SetDirection(visualsTransform.localScale.x);
                }
            }
        }
    }

    private void Dash()
    {
        UpdateStatusText("Dash");
        animator.SetTrigger("Dash");
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * dashSpeed;
        Debug.Log("Dash!");
        timeSinceLastAction = 0f;
    }

    private void Jump()
    {
        UpdateStatusText("Jump");
        animator.SetTrigger("Jump");
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        timeSinceLastJump = Random.Range(-2f, 2f);
        Debug.Log("Jump!");
    }

    private BossAttack GetAvailableRangedAttack(float distance)
    {
        BossAttack[] currentRangedPool;
        string phaseInfo;

        if (currentPhase == 1) { currentRangedPool = rangedPhase1Attacks; phaseInfo = "Phase 1"; }
        else if (currentPhase == 2) { currentRangedPool = rangedPhase2Attacks; phaseInfo = "Phase 2"; }
        else { currentRangedPool = rangedPhase3Attacks; phaseInfo = "Phase 3"; }

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine($"Searching ranged skills ({phaseInfo})... Player distance: {distance}");

        List<BossAttack> validAttacks = new List<BossAttack>();
        foreach (var attack in currentRangedPool)
        {
            logBuilder.Append($"  - Checking '{attack.attackName}' (range: {attack.minRange}-{attack.maxRange})... ");
            if (distance >= attack.minRange && distance <= attack.maxRange)
            {
                validAttacks.Add(attack);
                logBuilder.AppendLine("In range!");
            }
            else
            {
                logBuilder.AppendLine("Out of range.");
            }
        }

        if (validAttacks.Count == 0)
        {
            logBuilder.AppendLine("No ranged skills available at current distance.");
            Debug.Log(logBuilder.ToString());
            return null;
        }
        else
        {
            BossAttack chosenAttack = GetRandomAttack(validAttacks.ToArray());
            logBuilder.AppendLine($"Chosen '{chosenAttack.attackName}' from {validAttacks.Count} valid skills.");
            Debug.Log(logBuilder.ToString());
            return chosenAttack;
        }
    }

    private BossAttack GetRandomAttack(BossAttack[] attackArray)
    {
        if (attackArray == null || attackArray.Length == 0) return null;
        return attackArray[Random.Range(0, attackArray.Length)];
    }

    #endregion

    #region --- State Changes and Damage ---

    public void EngageTarget(Transform newTarget)
    {
        player = newTarget;
        currentState = State.Fighting;
        Debug.Log("Target found, entering combat!");
    }

    public void DisengageTarget()
    {
        player = null;
        currentState = State.Returning;
        Debug.Log("Target lost, returning to start position!");
    }

    public void TakeDamage(float damage)
    {
        if (currentState == State.Death) return;
        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
            int previousPhase = currentPhase;
            if (currentHealth / maxHealth <= phase3Threshold) currentPhase = 3;
            else if (currentHealth / maxHealth <= phase2Threshold) currentPhase = 2;

            if (currentPhase > previousPhase)
            {
                StartCoroutine(PhaseTransitionPushback());
            }
        }
    }

    private void Die()
    {
        currentState = State.Death;
        UpdateStatusText("Defeated");
        animator.SetTrigger("Death");
        Debug.Log("Boss defeated!");

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5f);
    }

    private IEnumerator PhaseTransitionPushback()
    {
        State originalState = currentState;
        currentState = State.Dormant;
        UpdateStatusText("Phase Transition!");
        animator.Play("Pushback");
        Debug.Log($"Phase {currentPhase} transition, pushback!");

        yield return new WaitForSeconds(0.5f);

        if (player != null && playerHealth != null)
        {
            Vector2 pushDirection = (player.position - transform.position).normalized;
            player.GetComponent<Rigidbody2D>().AddForce(pushDirection * 25f, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1f);
        currentState = originalState;
        UpdateStatusText("Fighting");
    }

    #endregion

    #region --- Utility Functions ---
    private void LookAtPlayer()
    {
        if (player == null) return;
        LookAtPosition(player.position);
    }

    private void LookAtPosition(Vector3 targetPosition)
    {
        if (visualsTransform == null)
        {
            Debug.LogError("Visuals Transform not assigned in Inspector!");
            return;
        }

        bool shouldFaceRight = (targetPosition.x > transform.position.x);
        bool isCurrentlyFacingRight = (visualsTransform.localScale.x > 0);

        if (shouldFaceRight != isCurrentlyFacingRight)
        {
            Vector3 currentScale = visualsTransform.localScale;
            currentScale.x *= -1;
            visualsTransform.localScale = currentScale;
        }
    }

    private void UpdateStatusText(string actionName)
    {
        if (statusText == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Phase: {currentPhase}");
        sb.AppendLine($"State: {currentState.ToString()}");
        sb.AppendLine($"Action: {actionName}");

        statusText.text = sb.ToString();
    }
    #endregion
}
