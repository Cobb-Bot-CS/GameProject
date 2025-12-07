using System.Collections;//主要是为了使用 IEnumerator
using System.Collections.Generic;
using System.Text;
//用来在 Boss 头顶或者 UI 上显示它当前在干嘛（Thinking, Moving, Attacking 等），方便调试。
using TMPro;
using UnityEngine;
using UnityEngine.UI; //控制 Boss 的血条显示

/*
 * Filename: BossAI_Advanced.cs
 * Developer: Qiwei Liang
 * Purpose: This file is to controll boss
 */

public enum SpawnLocation { Self, OnPlayer } // Define two spawn locations: self or on the player

//让这个类出现在 Inspector 里，可以直接在 Unity 面板中配置每一个技能
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
    public SpawnLocation spawnLocation; //attack form where
    public float minRange = 0f;
    public float maxRange = 5f;
    public float damage = 10f; // Damage for melee attacks
    public float cooldown = 2f;
}

public class BossAI_Advanced : EnemyBase
{
    [Header("UI & Debugging")]
    [SerializeField] private TextMeshProUGUI statusText; // Text component for showing current state
    [Tooltip("Projectile spawn point such as fireball position")] //在inspector点击时有提示
    [SerializeField] private Transform firePoint; //location of 子物体，生成fire

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
    [SerializeField] private CharacterHealth playerHealth;

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

    [Tooltip("Visual child object that should be flipped")]
    [SerializeField] private Transform visualsTransform;

    void Start()
    {
        initialPosition = transform.position;
        currentState = State.Dormant;
        UpdateStatusText("Dormant");
        animator.Play("Bored");
        currentHealth = maxHealth;

    }

    void Update()
    {
        if (currentState == State.Death) return;

        // State machine logic
        switch (currentState)
        {
            case State.Dormant:
                BoredState();
                break;
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
        animator.Play("Idle");
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
        //是否开始下一次攻击和跳跃
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

        //当 Boss 冷却中 + 正在移动 → 显示：Moving
        if (timeSinceLastAction <= actionCooldown && currentState == State.Fighting && rb.linearVelocity.magnitude > 0.1f)
        {
            UpdateStatusText("Moving");
        }

        //Boss 走得越快 → Speed 越大 → 播放走路动画越快
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
            //Instantiate Unity 用来生成一个物体 的函数，Quaternion.identity是默认旋转
            Instantiate(attack.castVFX, transform.position, Quaternion.identity);
        }

        rb.linearVelocity = Vector2.zero;//在攻击时停止移动
        animator.SetFloat("AttackIndex", attack.animationIndex);
        animator.SetTrigger("Attack");
        timeSinceLastAction = 0f;
    }

    public void SpawnAttackPrefab() //判断攻击类型，处理近战伤害，生成远程攻击的 prefab，设置伤害脚本，设置 projectile 移动方向
    {
        if (currentAttack == null)
        {
            Debug.LogError("SpawnAttackPrefab was called, but currentAttack is null!");
            return;
        }

        if (currentAttack.attackPrefab == null)//判断是否melee attack，因为远程攻击有prefab
        {
            Debug.Log($"Performing melee damage check: {currentAttack.attackName}...");
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentAttack.maxRange, LayerMask.GetMask("Player"));//圆形检测
            foreach (Collider2D hit in hits) //hits 是一个 Collider2D 数组，里面是所有在攻击范围内的玩家碰撞体。 foreach：对每一个命中的对象做一次检查和处理。
            {
                CharacterHealth playerHealth = hit.GetComponent<CharacterHealth>();
                if (playerHealth != null)
                {
                    playerHealth.CharacterHurt((int)((EnemyBase)this).GetMeleeDamage());
                    if (currentAttack.hitVFX != null)  //Boss 近战命中玩家时，会在玩家身上生成一个命中特效。
                    {
                        Instantiate(currentAttack.hitVFX, hit.transform.position, Quaternion.identity);
                    }
                    break;
                }
            }
        }
        else
        {
            //创建一个变量 projectile，用于保存待会实例化出来的“远程攻击物体”
            GameObject projectile = null;

            switch (currentAttack.spawnLocation)
            {
                case SpawnLocation.Self: //远程攻击(fire ball)需要从 Boss 身上发射出来。
                    if (firePoint == null) //firePoint 是 Boss 脸上的一个点（例如嘴、手）,用于准确发射 projectile,r如果没生成;
                    {
                        Debug.LogError("Skill set to 'Self' but no FirePoint specified! Using boss center instead.");
                        projectile = Instantiate(currentAttack.attackPrefab, transform.position, Quaternion.identity); //用 transform.position（Boss 中心）代替，避免技能无法生成。
                    }
                    else
                    {
                        projectile = Instantiate(currentAttack.attackPrefab, firePoint.position, firePoint.rotation);
                    }
                    Debug.Log($"Spawned projectile from fire point: {currentAttack.attackPrefab.name}");
                    break;

                case SpawnLocation.OnPlayer:   //在玩家脚下生成技能
                    if (player != null)        //用于 “范围攻击，firestorm，black hole，lighting
                    {
                        projectile = Instantiate(currentAttack.attackPrefab, player.position, Quaternion.identity);  //技能直接在玩家脚下出现
                        Debug.Log($"Spawned projectile on player: {currentAttack.attackPrefab.name}");
                    }
                    break;
            }

            if (projectile != null)  //表示 skill prefab 已经成功实例化
            {
                DamagePlayer damageScript = projectile.GetComponent<DamagePlayer>();  //DamagePlayer 是挂在 projectile 上的脚本,它负责：projectile 碰到玩家 → 玩家扣血播。放命中特效（hitVFX）
                if (damageScript != null)
                {
                    damageScript.Setup(currentAttack.damage, currentAttack.hitVFX); //Setup 会把：currentAttack.damage，currentAttack.hitVFX，传给 projectile。
                }
                //让 projectile 朝正确方向飞
                ProjectileMover mover = projectile.GetComponent<ProjectileMover>();
                if (mover != null && player != null)
                {
                    mover.SetDirection(visualsTransform.localScale.x);//SetDirection()，+1 → Boss 朝右，-1 → Boss 朝左，让 projectile。目的 朝向 Boss 面对的方向飞
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
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); //跳多高由 jumpForce 决定，Impulse = “瞬间爆发性的力”
        timeSinceLastJump = Random.Range(-2f, 2f); //这是个亮点！Boss 跳跃频率带随机性，有时候跳得快
        Debug.Log("Jump!");
    }

    private BossAttack GetAvailableRangedAttack(float distance) //根据玩家距离与 Boss 当前阶段，从对应的远程技能池中挑选所有“符合距离条件”的技能，再从中随机选一个返回。
    {
        BossAttack[] currentRangedPool;  //声明一个 BossAttack[] 类型的数组变量 currentRangedPool，用来保存“当前阶段要使用的远程技能池”
        string phaseInfo;       //用于记录当前是“Phase 1 / Phase 2 / Phase 3”，后面会写进日志里

        if (currentPhase == 1) { currentRangedPool = rangedPhase1Attacks; phaseInfo = "Phase 1"; }  //当前远程技能池 = 第一阶段的远程技能数组
        else if (currentPhase == 2) { currentRangedPool = rangedPhase2Attacks; phaseInfo = "Phase 2"; }
        else { currentRangedPool = rangedPhase3Attacks; phaseInfo = "Phase 3"; }

        var logBuilder = new StringBuilder();    //var：让编译器自动推断类型，这里是 StringBuilder。new StringBuilder()：创建一个新的 StringBuilder 实例。
        logBuilder.AppendLine($"Searching ranged skills ({phaseInfo})... Player distance: {distance}");

        List<BossAttack> validAttacks = new List<BossAttack>();  //用来存放 所有“距离符合条件”的远程技能，后面会从这个列表中随机选一个
        foreach (var attack in currentRangedPool)    //foreach：遍历数组 currentRangedPool 里的每一个元素。对当前阶段所有远程技能，一个一个检查是否“适合当前距离”。
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
            //validAttacks.ToArray()：把 List 转成数组，传给 GetRandomAttack()
            //GetRandomAttack()：从可用技能列表里随机挑选一个技能。
            //BossAttack chosenAttack：保存最终被选中的远程攻击技能。
            BossAttack chosenAttack = GetRandomAttack(validAttacks.ToArray());
            logBuilder.AppendLine($"Chosen '{chosenAttack.attackName}' from {validAttacks.Count} valid skills.");
            Debug.Log(logBuilder.ToString());
            return chosenAttack;
        }
    }

    private BossAttack GetRandomAttack(BossAttack[] attackArray) //在一堆技能里面随机挑一个出来
    {
        if (attackArray == null || attackArray.Length == 0) return null;
        return attackArray[Random.Range(0, attackArray.Length)];  //shuffle意思是打乱，random是随机pickone
    }

    #endregion

    #region --- State Changes and Damage ---

    public void EngageTarget(Transform newTarget) //newTarget 是玩家的 Transform，也就是玩家位置
    {
        player = newTarget;
        currentState = State.Fighting;
        Debug.Log("Target found, entering combat!");
    }

    public void DisengageTarget() //离开战斗
    {
        player = null;
        currentState = State.Returning;
        Debug.Log("Target lost, returning to start position!");
    }
    public override float GetMeleeDamage()
    {
        Debug.Log("[BossAI] OVERRIDDEN melee damage");
        return 20f;
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
            int previousPhase = currentPhase;  //比较是否进入下一阶段
            if (currentHealth / maxHealth <= phase3Threshold) currentPhase = 3;
            else if (currentHealth / maxHealth <= phase2Threshold) currentPhase = 2;

            if (currentPhase > previousPhase) //如果阶段提升，触发特殊推开攻击pushback
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
        rb.bodyType = RigidbodyType2D.Kinematic;  //不受物理影响
        GetComponent<Collider2D>().enabled = false;  //禁用碰撞体

        // Trigger win screen
        WinScreen winScreen = FindAnyObjectByType<WinScreen>();
        if (winScreen != null)
        {
            winScreen.ShowWinScreen();
        }
        else
        {
            Debug.LogWarning("No WinScreen found!");
        }

        Destroy(gameObject, 5f);//5秒后销毁
    }
    private IEnumerator PhaseTransitionPushback() //IEnumerator 是协程（Coroutine）函数，可以等待暂停分步骤执行不卡死游戏。核心是yield return 是“暂停点”
    {
        //Boss 变身时让它停止动作，播放一个“推开玩家”的动画。
        State originalState = currentState;
        currentState = State.Dormant;
        UpdateStatusText("Phase Transition!");
        animator.Play("Pushback");
        Debug.Log($"Phase {currentPhase} transition, pushback!");

        yield return new WaitForSeconds(0.5f);

        //推开玩家
        if (player != null && playerHealth != null)
        {
            Vector2 pushDirection = (player.position - transform.position).normalized;
            player.GetComponent<Rigidbody2D>().AddForce(pushDirection * 25f, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1f);
        //回到战斗
        currentState = originalState;
        UpdateStatusText("Fighting");
    }

    #endregion

    #region --- Utility Functions ---
    private void LookAtPlayer() //给玩家写的专用快捷方式（内含防错机制）
    {
        if (player == null) return;
        LookAtPosition(player.position);//ever face to player
    }

    private void LookAtPosition(Vector3 targetPosition) //给所有情况都能用的通用数学函数
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