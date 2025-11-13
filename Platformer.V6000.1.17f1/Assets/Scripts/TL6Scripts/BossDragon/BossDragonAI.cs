
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace BossOne
{
    public class BossDragonAI : MonoBehaviour
    {
        /* ====================== ENUMS & STATES ====================== */

        private enum State
        {
            Dormant,
            Returning,
            Idle,
            Fighting,
            Dizzy,
            Death,
            Victory,
            Phase2Transition
        }

        private enum Phase2SubState
        {
            None,
            FastFireball,
            FlyingKick,
            RetreatAfterKick,
            ChargeAttack
        }

        private State currentState;
        private Phase2SubState currentPhase2SubState = Phase2SubState.None;



        /* ====================== INSPECTOR FIELDS ====================== */

        [Header("UI & Debugging")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Transform firePoint;


        [Header("Core References")]
        [SerializeField] private Transform player;
        [SerializeField] private BossArenaController arenaController;
        [SerializeField] private Transform visualsTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CharacterHealthScript playerHealth;




        [Header("Stats")]
        public Slider healthBar;
        public float maxHealth = 2000f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float dashSpeed = 4f;
        [SerializeField] private float dizzyDuration = 1f;

        private float currentHealth;


        [Header("Phase Settings")]
        [Tooltip("当前阶段：0 = 未开始, 1 = 阶段1, 2 = 阶段2")]
        [SerializeField] private int currentPhase = 0;

        [Tooltip("血量比例低于此值进入第二阶段，例如 0.7 = 70%")]
        [SerializeField] private float phase2Threshold = 0.7f;

        private bool phaseTransitioning = false;
        private Coroutine phase2BehaviorCoroutine;
        private Vector3 initialPosition;


        [Header("Phase 2 Behavior Parameters")]
        [Tooltip("阶段2高频火球持续时间")]
        [SerializeField] private float phase2FastFireballDuration = 2f;

        [Tooltip("阶段2高频火球间隔")]
        [SerializeField] private float phase2FastFireballInterval = 0.2f;

        [Tooltip("阶段2飞踢触发距离（Boss 距离玩家小于该值时可以飞踢）")]
        [SerializeField] private float phase2FlyKickRange = 3f;

        [Tooltip("飞踢或冲撞后，Boss 撤退的距离")]
        [SerializeField] private float phase2RetreatAfterKickDistance = 2f;

        [Tooltip("冲撞攻击允许的最小距离")]
        [SerializeField] private float phase2ChargeAttackMinRange = 1f;

        [Tooltip("冲撞攻击允许的最大距离")]
        [SerializeField] private float phase2ChargeAttackMaxRange = 3f;

        [Tooltip("阶段2时 Boss 期望飞行高度（世界坐标Y）")]
        [SerializeField] private float phase2FlyHeight = 2f;


        [Header("Phase 1 & Generic AI Parameters")]
        [SerializeField] private float meleeRange = 1f;
        [SerializeField] private float rangedAttackMinRange = 2f;
        [SerializeField] private float rangedAttackMaxRange = 3f;
        [SerializeField] private float retreatDistance = 2f;
        [SerializeField] private float actionCooldown = 1f;

        private float timeSinceLastAction = 0f;


        [Header("Attacks")]
        [Tooltip("阶段1近战攻击数据")]
        [SerializeField] private DragonAttack meleeAttack_Phase1;

        [Tooltip("阶段1普通远程火球")]
        [SerializeField] private DragonAttack rangedFireball_Phase1_Normal;

        [Tooltip("阶段1受击后撤退时使用的火球")]
        [SerializeField] private DragonAttack rangedFireball_Phase1_Retreat;

        [Tooltip("阶段2高频追踪火球")]
        [SerializeField] private DragonAttack rangedFireball_Phase2_FastTracking;

        [Tooltip("阶段2飞踢攻击")]
        [SerializeField] private DragonAttack flyingKick_Phase2;

        [Tooltip("阶段2冲撞攻击")]
        [SerializeField] private DragonAttack chargeAttack_Phase2;



        /* ====================== RUNTIME FIELDS ====================== */

        private DragonAttack currentAttack;



        /* ====================== UNITY CYCLE ====================== */

        private void Start()
        {
            initialPosition = transform.position;
            currentState = State.Dormant;

            currentHealth = maxHealth;
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }

            UpdateStatusText("Dormant");
        }



        private void Update()
        {
            if (currentState == State.Death ||
                currentState == State.Victory ||
                currentState == State.Phase2Transition)
            {
                return;
            }

            // 玩家丢失时自动脱战（但不重置阶段）
            if (player == null &&
                currentState != State.Dormant &&
                currentState != State.Idle &&
                currentState != State.Returning)
            {
                DisengageTarget();
                return;
            }

            HandleFacingDirection();

            switch (currentState)
            {
                case State.Idle:
                    animator.Play("Idle");
                    UpdateStatusText("Idle");
                    break;

                case State.Returning:
                    HandleReturningState();
                    break;

                case State.Fighting:
                    HandleFightingState();
                    break;

                case State.Dizzy:
                    UpdateStatusText("Dizzy!");
                    break;
            }

            if (currentState == State.Fighting || currentState == State.Returning)
            {
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
            }

            timeSinceLastAction += Time.deltaTime;
        }



        /* ====================== FACING & MOVEMENT ====================== */

        private void HandleFacingDirection()
        {
            if (player == null || currentState == State.Dizzy)
                return;

            bool shouldFaceRight = (player.position.x > transform.position.x);
            bool isFacingRight = (visualsTransform.localScale.x > 0);

            if (shouldFaceRight != isFacingRight)
            {
                Vector3 scale = visualsTransform.localScale;
                scale.x *= -1;
                visualsTransform.localScale = scale;
            }
        }



        private void MoveTowardsPlayer()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);
            float stopDistance = 1.5f;

            if (distance > stopDistance)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
                UpdateStatusText("Moving to player");
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                UpdateStatusText("Reached player range");
            }
        }



        private void HandleReturningState()
        {
            UpdateStatusText("Returning");

            Vector3 targetPos = initialPosition;

            if (player != null)
            {
                targetPos = new Vector3(initialPosition.x, transform.position.y, transform.position.z);
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // 回到初始点且没有玩家 → Idle
            if (player == null && Vector2.Distance(transform.position, initialPosition) < 0.1f)
            {
                currentState = State.Idle;
                rb.linearVelocity = Vector2.zero;
            }
            // 回到初始点且有玩家 → 继续战斗
            else if (player != null && Vector2.Distance(transform.position, initialPosition) < 0.1f)
            {
                currentState = State.Fighting;
                rb.linearVelocity = Vector2.zero;

                if (currentPhase == 2 && phase2BehaviorCoroutine == null)
                {
                    phase2BehaviorCoroutine = StartCoroutine(Phase2BehaviorCoroutine());
                }
            }
        }



        /* ====================== FIGHTING LOGIC ====================== */

        private void HandleFightingState()
        {
            if (player == null)
            {
                DisengageTarget();
                return;
            }

            if (phaseTransitioning)
                return;

            // 玩家死亡 → Boss 胜利
            if (playerHealth != null && playerHealth.GetHealth() <= 0)

            {
                currentState = State.Victory;
                animator.SetTrigger("Victory");
                rb.linearVelocity = Vector2.zero;
                UpdateStatusText("Victory!");
                StopAllCoroutines();
                return;
            }

            if (currentPhase == 1)
            {
                HandlePhase1Behavior();
            }
            else if (currentPhase == 2)
            {
                // 行为在协程中循环，如果协程挂了就重启
                if (phase2BehaviorCoroutine == null && !phaseTransitioning)
                {
                    phase2BehaviorCoroutine = StartCoroutine(Phase2BehaviorCoroutine());
                }
            }
        }



        private void HandlePhase1Behavior()
        {
            float distance = Vector2.Distance(transform.position, player.position);
            // 如果距离玩家太近，则后退保持安全距离
            if (distance < retreatDistance)
            {
                Vector2 away = ((Vector2)transform.position - (Vector2)player.position).normalized;
                rb.linearVelocity = away * moveSpeed;
                UpdateStatusText("Retreating to safe range");
                return;
            }

            if (timeSinceLastAction > actionCooldown)
            {
                if (distance <= meleeRange)
                {
                    rb.linearVelocity = Vector2.zero;
                    PerformAttack(meleeAttack_Phase1);
                }
                else if (distance > rangedAttackMinRange && distance < rangedAttackMaxRange)
                {
                    MoveTowardsPlayer();
                    PerformAttack(rangedFireball_Phase1_Normal);
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }



        /* ====================== PHASE 2 MASTER COROUTINE ====================== */

        private IEnumerator Phase2BehaviorCoroutine()
        {
            while (currentState == State.Fighting && currentPhase == 2)
            {
                if (player == null) { DisengageTarget(); yield break; }

                // 0. 确保在空中
                animator.SetBool("IsFlying", true);
                yield return StartCoroutine(FlyToHeight(phase2FlyHeight, moveSpeed));
                if (player == null) yield break;

                /* ------- 1. 冲撞攻击 ------- */
                currentPhase2SubState = Phase2SubState.ChargeAttack;
                UpdateStatusText("Phase 2: Charge Attack");
                yield return StartCoroutine(ChargeAttackRoutine(
                   chargeAttack_Phase2,
                   phase2ChargeAttackMinRange,
                   phase2ChargeAttackMaxRange
                ));
                if (player == null) yield break;

                /* ------- 2. 冲撞后小撤退 ------- */
                currentPhase2SubState = Phase2SubState.RetreatAfterKick;
                UpdateStatusText("Phase 2: Retreat after Charge");
                yield return StartCoroutine(RetreatFromPlayerRoutine(phase2RetreatAfterKickDistance * 0.7f));
                if (player == null) yield break;

                /* ------- 3. 空中高频火球 ------- */
                currentPhase2SubState = Phase2SubState.FastFireball;
                UpdateStatusText("Phase 2: Fast Fireballs");
                yield return StartCoroutine(FastFireballAttackRoutine(
                   phase2FastFireballDuration,
                   phase2FastFireballInterval
                ));
                if (player == null) yield break;

                /* ------- 4. 飞踢（从空中突进） ------- */
                if (flyingKick_Phase2 != null)
                {
                    currentPhase2SubState = Phase2SubState.FlyingKick;
                    UpdateStatusText("Phase 2: Flying Kick");
                    yield return StartCoroutine(FlyToPositionAndAttack(
                       player.position,
                       flyingKick_Phase2,
                       phase2FlyKickRange
                    ));
                    if (player == null) yield break;
                }

                /* ------- 5. 飞踢后撤退，回到安全距离 ------- */
                currentPhase2SubState = Phase2SubState.RetreatAfterKick;
                UpdateStatusText("Phase 2: Retreat after Kick");
                yield return StartCoroutine(RetreatFromPlayerRoutine(phase2RetreatAfterKickDistance));
                if (player == null) yield break;

                // 循环冷却
                currentPhase2SubState = Phase2SubState.None;
                yield return new WaitForSeconds(actionCooldown);
            }

            phase2BehaviorCoroutine = null;
        }



        /* ====================== PHASE 2 HELPERS ====================== */

        private IEnumerator FlyToHeight(float targetY, float speed)
        {
            while (Mathf.Abs(transform.position.y - targetY) > 0.1f &&
                   player != null &&
                   currentState == State.Fighting)
            {
                Vector2 dir = (player.position - transform.position);
                dir.y = (targetY > transform.position.y) ? 1f : -1f;
                dir.x = (player.position.x > transform.position.x) ? 1f : -1f;

                rb.linearVelocity = dir.normalized * speed;
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.magnitude) / speed);
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
        }



        private IEnumerator FastFireballAttackRoutine(float duration, float interval)
        {
            if (rangedFireball_Phase2_FastTracking == null)
            {
                Debug.LogError("Phase2 Fireball 未绑定（rangedFireball_Phase2_FastTracking）。");
                yield break;
            }

            float timer = 0f;

            while (timer < duration &&
                   currentState == State.Fighting &&
                   currentPhase == 2)
            {
                if (player == null) yield break;

                ProjectileTracking originalTracking = rangedFireball_Phase2_FastTracking.projectileTracking;
                rangedFireball_Phase2_FastTracking.projectileTracking = ProjectileTracking.ToPlayerDirectly;

                PerformAttack(rangedFireball_Phase2_FastTracking);
                Debug.Log($"🔥 [Phase2 Fireball] Fired at {player.position}");

                rangedFireball_Phase2_FastTracking.projectileTracking = originalTracking;

                yield return new WaitForSeconds(interval);
                timer += interval;

                while (currentState == State.Dizzy)
                    yield return null;
            }
        }



        private IEnumerator FlyToPositionAndAttack(Vector2 targetPos, DragonAttack attack, float triggerRange)
        {
            // 从空中飞向玩家附近
            while (Vector2.Distance(transform.position, targetPos) > triggerRange &&
                   currentState == State.Fighting)
            {
                if (player == null) { DisengageTarget(); yield break; }

                Vector2 desired = new Vector2(targetPos.x, phase2FlyHeight);
                rb.linearVelocity = (desired - (Vector2)transform.position).normalized * moveSpeed;
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x) / moveSpeed);

                yield return null;

                while (currentState == State.Dizzy)
                    yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);

            if (player == null) { DisengageTarget(); yield break; }

            PerformAttack(attack);
            yield return new WaitForSeconds(attack.cooldown);

            while (currentState == State.Dizzy)
                yield return null;
        }



        private IEnumerator RetreatFromPlayerRoutine(float retreatDist)
        {
            if (player == null) { DisengageTarget(); yield break; }

            Vector2 away = ((Vector2)transform.position - (Vector2)player.position).normalized;
            Vector2 target = new Vector2(
               transform.position.x + away.x * retreatDist,
               phase2FlyHeight
            );

            while (Vector2.Distance(transform.position, target) > 0.5f &&
                   currentState == State.Fighting)
            {
                if (player == null) { DisengageTarget(); yield break; }

                rb.linearVelocity = (target - (Vector2)transform.position).normalized * dashSpeed;
                animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.magnitude) / dashSpeed);
                yield return null;

                while (currentState == State.Dizzy)
                    yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            yield return new WaitForSeconds(actionCooldown);
        }



        private IEnumerator ChargeAttackRoutine(DragonAttack attack, float minRange, float maxRange)
        {
            if (attack == null || player == null)
                yield break;

            // 1. 对齐 Y 高度
            float targetY = player.position.y;
            yield return StartCoroutine(FlyToHeight(targetY, moveSpeed * 1.5f));

            if (player == null) yield break;

            // 2. 调整 X 到指定范围
            float timer = 0f;
            float maxAdjustTime = 2f;
            float distance = Vector2.Distance(transform.position, player.position);

            while ((distance < minRange || distance > maxRange) &&
                   timer < maxAdjustTime &&
                   currentState == State.Fighting)
            {
                if (player == null) { DisengageTarget(); yield break; }

                MoveTowardsPlayer();
                distance = Vector2.Distance(transform.position, player.position);

                timer += Time.deltaTime;
                yield return null;

                while (currentState == State.Dizzy)
                    yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            if (player == null) { DisengageTarget(); yield break; }

            // 3. 蓄力 + 冲撞
            animator.SetTrigger("ChargePrepare");
            UpdateStatusText("Phase 2: Charging");
            yield return new WaitForSeconds(0.3f);

            PerformAttack(attack);      // 触发动画，由动画事件调用 PerformChargeAttack()
            yield return new WaitForSeconds(attack.cooldown);

            while (currentState == State.Dizzy)
                yield return null;
        }



        /* ====================== ATTACK EXECUTION ====================== */

        private void PerformAttack(DragonAttack attack)
        {
            if (attack == null ||
                currentState == State.Dizzy ||
                phaseTransitioning ||
                player == null)
            {
                return;
            }

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



        // 动画事件调用：生成子弹 / 进行近战命中检测
        public void SpawnAttackPrefab()
        {
            if (currentAttack == null || player == null)
            {
                Debug.LogWarning("SpawnAttackPrefab called but currentAttack or player is null.");
                return;
            }

            // 近战：不生成火球，直接检测范围
            if (currentAttack.attackPrefab == null)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                   transform.position,
                   currentAttack.maxRange,
                   LayerMask.GetMask("Player")
                );

                foreach (Collider2D hit in hits)
                {
                    if (!hit.CompareTag("Player")) continue;

                    PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                    if (ph != null)
                    {
                        ph.TakeDamage(currentAttack.damage);

                        if (currentAttack.pushForce > 0f)
                        {
                            Vector2 pushDir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                            hit.attachedRigidbody?.AddForce(pushDir * currentAttack.pushForce, ForceMode2D.Impulse);
                        }
                    }

                    if (currentAttack.hitVFX != null)
                    {
                        Instantiate(currentAttack.hitVFX, hit.transform.position, Quaternion.identity);
                    }

                    break;
                }

                return;
            }

            // 远程：生成火球
            Vector2 spawnPos = transform.position;

            if (currentAttack.spawnLocation == SpawnLocation.Self &&
                firePoint != null)
            {
                spawnPos = firePoint.position;
            }
            else if (currentAttack.spawnLocation == SpawnLocation.OnPlayer &&
                     player != null)
            {
                spawnPos = player.position;
            }

            GameObject go = Instantiate(currentAttack.attackPrefab, spawnPos, Quaternion.identity);
            DragonProjectile proj = go.GetComponent<DragonProjectile>();

            if (proj != null)
            {
                float facing = visualsTransform.localScale.x > 0 ? 1f : -1f;

                proj.Setup(
                   currentAttack.damage,
                   currentAttack.hitVFX,
                   currentAttack.explosionVFX,
                   player,
                   currentAttack.projectileTracking,
                   facing
                );

                // 给非追踪弹一个初速度；追踪弹会在 DragonProjectile 里自动调整
                if (currentAttack.projectileTracking == ProjectileTracking.None)
                {
                    Vector2 dir = new Vector2(facing, 0f);
                    proj.SetInitialVelocity(dir * proj.speed);
                }
                else if (currentAttack.projectileTracking == ProjectileTracking.ToPlayerDirectly)
                {
                    Vector2 dir = ((Vector2)player.position - spawnPos).normalized;
                    proj.SetInitialVelocity(dir * proj.speed);
                }
            }
        }



        // 动画事件调用：执行实际的冲撞移动
        public void PerformChargeAttack()
        {
            if (player == null) return;

            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * dashSpeed;

            Debug.Log("⚡ Boss Charge Attack Movement");
        }



        /* ====================== ENGAGE / DISENGAGE ====================== */

        public void EngageTarget(Transform target)
        {
            player = target;
            playerHealth = target.GetComponent<CharacterHealthScript>();


            if (currentPhase == 0)
            {
                currentPhase = 1;
            }

            currentState = State.Fighting;
            UpdateStatusText("Engaged");

            if (currentPhase == 2 && phase2BehaviorCoroutine == null)
            {
                phase2BehaviorCoroutine = StartCoroutine(Phase2BehaviorCoroutine());
            }

            Debug.Log($"🐉 Boss engaged player. Phase = {currentPhase}");
        }



        public void DisengageTarget()
        {
            StopAllCoroutines();
            phase2BehaviorCoroutine = null;

            player = null;
            rb.linearVelocity = Vector2.zero;

            currentState = State.Returning;
            UpdateStatusText("Disengaged - Returning");

            Debug.Log("Boss disengaged and returning to origin.");
        }



        /* ====================== DAMAGE & STATES ====================== */

        public void TakeDamage(float damage)
        {
            if (currentState == State.Death ||
                currentState == State.Dizzy ||
                phaseTransitioning ||
                player == null)
            {
                return;
            }

            currentHealth -= damage;
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }

            if (currentHealth <= 0f)
            {
                Die();
                return;
            }

            animator.SetTrigger("Hurt");

            // 阶段切换检测
            if (currentPhase == 1 &&
                (currentHealth / maxHealth) <= phase2Threshold)
            {
                StartCoroutine(PhaseTransitionToPhase2());
                return;
            }

            // 阶段2受击 → 短暂眩晕
            if (currentPhase == 2)
            {
                StartCoroutine(DizzyCoroutine());
            }
            // 阶段1受击 → 小撤退 + 喷火
            else
            {
                StartCoroutine(RetreatAndAttackAfterHit());
            }
        }



        private IEnumerator DizzyCoroutine()
        {
            if (currentState == State.Dizzy)
                yield break;

            State prev = currentState;
            currentState = State.Dizzy;

            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsDizzy", true);

            if (phase2BehaviorCoroutine != null)
            {
                StopCoroutine(phase2BehaviorCoroutine);
                phase2BehaviorCoroutine = null;
            }

            yield return new WaitForSeconds(dizzyDuration);

            animator.SetBool("IsDizzy", false);
            currentState = prev;

            if (currentPhase == 2 &&
                currentState == State.Fighting &&
                phase2BehaviorCoroutine == null)
            {
                phase2BehaviorCoroutine = StartCoroutine(Phase2BehaviorCoroutine());
            }
        }



        private IEnumerator RetreatAndAttackAfterHit()
        {
            if (player == null) { DisengageTarget(); yield break; }

            State prev = currentState;
            currentState = State.Returning;

            UpdateStatusText("Retreat after hit");
            rb.linearVelocity = Vector2.zero;

            Vector2 away = ((Vector2)transform.position - (Vector2)player.position).normalized;

            float start = Time.time;
            float duration = 1f;

            while (Time.time < start + duration && player != null)
            {
                rb.linearVelocity = new Vector2(away.x * dashSpeed, rb.linearVelocity.y);
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;

            if (player != null && rangedFireball_Phase1_Retreat != null)
            {
                PerformAttack(rangedFireball_Phase1_Retreat);
                yield return new WaitForSeconds(rangedFireball_Phase1_Retreat.cooldown);
            }

            currentState = prev;
        }



        private IEnumerator PhaseTransitionToPhase2()
        {
            phaseTransitioning = true;
            currentState = State.Phase2Transition;

            UpdateStatusText("Phase 2 Transition");
            animator.SetTrigger("PhaseChange");
            rb.linearVelocity = Vector2.zero;

            // 简单的后退上升效果
            if (player != null)
            {
                Vector2 away = ((Vector2)transform.position - (Vector2)player.position).normalized;
                float start = Time.time;
                float duration = 1.5f;

                while (Time.time < start + duration && player != null)
                {
                    rb.linearVelocity = new Vector2(away.x * dashSpeed, away.y * dashSpeed);
                    yield return null;
                }
            }

            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(3f);

            currentPhase = 2;
            phaseTransitioning = false;
            currentState = State.Fighting;

            animator.SetBool("IsFlying", true);
            UpdateStatusText("Phase 2 - Fighting");

            if (phase2BehaviorCoroutine != null)
                StopCoroutine(phase2BehaviorCoroutine);

            phase2BehaviorCoroutine = StartCoroutine(Phase2BehaviorCoroutine());

            Debug.Log("🐉 Boss entered Phase 2.");
        }



        private void Die()
        {
            currentState = State.Death;
            UpdateStatusText("Defeated");

            animator.SetTrigger("Death");
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StopAllCoroutines();
            Destroy(gameObject, 5f);

            Debug.Log("🐉 Boss Dragon defeated.");
        }



        /* ====================== DEBUG UI ====================== */

        private void UpdateStatusText(string action)
        {
            if (statusText == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Phase: {currentPhase}");
            sb.AppendLine($"State: {currentState}");
            sb.AppendLine($"SubState: {currentPhase2SubState}");
            sb.AppendLine($"Action: {action}");

            statusText.text = sb.ToString();
        }
    }
}
