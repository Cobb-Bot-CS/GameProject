/*
 * DragonAttack.cs
 *
 * Summary:
 *   Defines the data structure for dragon attacks.
 *   Each attack may include animation, projectile prefab, visual effects, and combat parameters.
 *
 * Author: Qiwei Liang
 * Date: 2025-11-11
 */

using UnityEngine;



namespace BossOne
{
    /// <summary>
    /// Enumeration defining where an attack spawns.
    /// </summary>
    public enum SpawnLocation
    {
        Self,       // 从Boss自身发射（火球、喷火）
        OnPlayer    // 在玩家位置生成（AOE或落雷类技能）
    }



    /// <summary>
    /// Enumeration defining projectile tracking type.
    /// </summary>
    public enum ProjectileTracking
    {
        None,               // 不追踪，按初速度前进
        ToPlayerDirectly,   // 实时追踪玩家位置
        ToPlayerOnGround    // 仅追踪玩家X轴（地面锁定）
    }



    /// <summary>
    /// Defines the parameters and assets for a single dragon attack.
    /// Used by BossDragonAI to perform attacks.
    /// </summary>
    [System.Serializable]
    public class DragonAttack
    {
        [Header("Basic Info")]
        public string attackName;                // 攻击名称
        [Tooltip("动画控制器中AttackIndex参数值，用于区分不同攻击动画")]
        public float animationIndex;             // 动画索引

        [Tooltip("攻击发射的预制体（为空则为近战）")]
        public GameObject attackPrefab;          // 远程攻击Prefab



        [Header("Visual Effects")]
        [Tooltip("施法时在Boss位置播放的特效")]
        public GameObject castVFX;
        [Tooltip("命中玩家时播放的特效")]
        public GameObject hitVFX;
        [Tooltip("火球击中地面或目标后的爆炸特效")]
        public GameObject explosionVFX;



        [Header("Attributes")]
        public SpawnLocation spawnLocation;
        public ProjectileTracking projectileTracking;
        public float minRange = 0f;
        public float maxRange = 5f;
        public float damage = 10f;
        public float cooldown = 2f;

        [Tooltip("玩家被击退的力度，仅用于近战或冲撞攻击")]
        public float pushForce = 0f;



        [Header("Phase Settings")]
        [Tooltip("是否在特定阶段具有更高优先级")]
        public bool isHighPriorityInPhase = false;
    }
}
