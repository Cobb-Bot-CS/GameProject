/*
 * BossArenaController.cs
 *
 * Summary:
 *   Handles the boss battle arena triggers.
 *   Detects player entering or leaving the arena area,
 *   and activates or deactivates the Boss Dragon AI accordingly.
 *
 * Author: Qiwei Liang
 * Date: 2025-11-11
 */

using UnityEngine;



namespace BossOne
{
    /// <summary>
    /// Controls when the boss engages or disengages the player.
    /// Typically attached to an invisible collider surrounding the boss arena.
    /// </summary>
    public class BossArenaController : MonoBehaviour
    {
        [Header("Boss Reference")]
        [Tooltip("Assign the BossDragonAI instance in this field.")]
        [SerializeField] private BossDragonAI bossDragonAI;



        /* ====================== UNITY CYCLE ====================== */

        private void Start()
        {
            if (bossDragonAI == null)
            {
                Debug.LogError("⚠️ BossArenaController: BossDragonAI reference not assigned!", this);
            }
        }



        /* ====================== TRIGGER EVENTS ====================== */

        /// <summary>
        /// Called when the player enters the arena trigger area.
        /// Engages the boss into active combat mode.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (bossDragonAI != null)
            {
                bossDragonAI.EngageTarget(other.transform);
                Debug.Log(" Player entered arena, boss engaged!");
            }
            else
            {
                Debug.LogWarning("⚠️ Player entered arena but BossDragonAI reference is missing!");
            }
        }



        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (bossDragonAI == null)
            {
                // 如果Boss已被销毁，则仅输出一次警告，不中断游戏
                Debug.Log("[Arena] Player exited, but Boss no longer exists (likely defeated).");
                return;
            }

            // 确保Boss仍存活才调用
            if (bossDragonAI.isActiveAndEnabled)
            {
                bossDragonAI.DisengageTarget();
                Debug.Log("[Arena] Player exited arena, boss disengaged.");
            }
        }

    }
}
