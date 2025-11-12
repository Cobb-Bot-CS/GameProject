/*
 * DragonProjectile.cs
 *
 * Summary:
 *   Handles projectile behavior for Boss Dragon attacks.
 *   Supports direct, ground-locked, and non-tracking fireballs.
 *   Manages collision, explosion, and damage delivery to the player.
 *
 * Author: Qiwei Liang
 * Date: 2025-11-11
 */

using UnityEngine;
using BossOne;



namespace BossOne
{
    public class DragonProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public float speed = 10f;            // 移动速度
        public float damage = 10f;           // 造成的伤害
        public float lifetime = 3f;          // 生命周期（自动销毁）
        public bool destroyOnHit = true;     // 碰撞后是否销毁

        private GameObject hitVFX;           // 命中特效
        private GameObject explosionVFX;     // 爆炸特效
        private Transform playerTarget;      // 追踪目标
        private ProjectileTracking trackingType;
        private Vector2 initialDirection;    // 初始方向（非追踪模式）

        private Rigidbody2D rb;



        /* ====================== UNITY CYCLE ====================== */

        void Awake()
        {
            // 尝试从当前物体获取 Rigidbody2D
            rb = GetComponent<Rigidbody2D>();

            // 如果 prefab 是 Static Sprite 且未手动加 Rigidbody2D，则自动添加一个
            if (rb == null)
            {
                Debug.Log("[DragonProjectile] Rigidbody2D not found on prefab, adding one automatically...");
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0; // 火球不受重力影响
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.freezeRotation = true;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            // 确保 Collider2D 存在
            var col = GetComponent<Collider2D>();
            if (col == null)
            {
                Debug.LogWarning("[DragonProjectile] Collider2D missing, adding a default CircleCollider2D.");
                var newCol = gameObject.AddComponent<CircleCollider2D>();
                newCol.isTrigger = true;
                newCol.radius = 0.25f;
            }
        }




        /* ====================== SETUP FUNCTIONS ====================== */

        /// <summary>
        /// Sets up projectile parameters based on attack data.
        /// </summary>
        /// <param name="dmg">Damage dealt by the projectile.</param>
        /// <param name="hVFX">Hit effect prefab.</param>
        /// <param name="eVFX">Explosion effect prefab.</param>
        /// <param name="target">Player transform for tracking.</param>
        /// <param name="tracking">Tracking type (None, Direct, or Ground).</param>
        /// <param name="faceDirection">Direction facing (1 = right, -1 = left).</param>
        public void Setup(float dmg, GameObject hVFX, GameObject eVFX,
                          Transform target, ProjectileTracking tracking, float faceDirection)
        {
            damage = dmg;
            hitVFX = hVFX;
            explosionVFX = eVFX;
            playerTarget = target;
            trackingType = tracking;

            // 依据Boss朝向决定初始方向
            initialDirection = (faceDirection > 0) ? Vector2.right : Vector2.left;
            transform.localScale = new Vector3(faceDirection, transform.localScale.y, transform.localScale.z);
        }



        /// <summary>
        /// Sets the initial velocity for non-tracking projectiles.
        /// </summary>
        public void SetInitialVelocity(Vector2 velocity)
        {
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
        }



        /* ====================== MOVEMENT LOGIC ====================== */

        private void FixedUpdate()
        {
            // --- 追踪模式 ---
            if (playerTarget != null && trackingType != ProjectileTracking.None)
            {
                Vector2 targetPosition = playerTarget.position;

                // 仅在X轴追踪的下落型火球
                if (trackingType == ProjectileTracking.ToPlayerOnGround)
                {
                    targetPosition.y = rb.position.y;
                }

                Vector2 directionToTarget = (targetPosition - rb.position).normalized;
                rb.linearVelocity = directionToTarget * speed;

                // 翻转视觉方向以匹配移动
                if (rb.linearVelocity.x != 0)
                {
                    float newDir = Mathf.Sign(rb.linearVelocity.x);
                    transform.localScale = new Vector3(newDir, transform.localScale.y, transform.localScale.z);
                }
            }

            // --- 非追踪模式 ---
            else if (rb != null && trackingType == ProjectileTracking.None)
            {
                if (rb.linearVelocity == Vector2.zero)
                {
                    rb.linearVelocity = initialDirection * speed;
                }
            }
        }



        /* ====================== COLLISION HANDLING ====================== */

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 仅对玩家与地面进行反应
            if (!other.CompareTag("Player") && !other.CompareTag("Ground"))
                return;

            // --- 击中玩家 ---
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);

                    if (hitVFX != null)
                    {
                        Instantiate(hitVFX, other.transform.position, Quaternion.identity);
                    }
                }
            }

            // --- 播放爆炸特效 ---
            if (explosionVFX != null)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
            }

            // --- 销毁火球 ---
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
