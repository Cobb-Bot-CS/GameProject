/*
 * DragonVisuals.cs
 *
 * Summary:
 *   Acts as the interface between Animator events and the BossDragonAI logic.
 *   Allows animation clips to trigger attacks and special actions through event calls.
 *
 * Author: Qiwei Liang
 * Date: 2025-11-11
 */

using UnityEngine;



namespace BossOne
{
    /// <summary>
    /// Receives animation events and triggers AI actions accordingly.
    /// </summary>
    public class DragonVisuals : MonoBehaviour
    {
        [Tooltip("Reference to the main BossDragonAI component.")]
        public BossDragonAI bossDragonAI;



        /// <summary>
        /// Called by animation event during attack animation to spawn projectiles.
        /// </summary>
        public void SpawnAttackPrefabEvent()
        {
            if (bossDragonAI != null)
            {
                bossDragonAI.SpawnAttackPrefab();
            }
            else
            {
                Debug.LogWarning(" DragonVisuals: Missing BossDragonAI reference for SpawnAttackPrefabEvent.");
            }
        }



        /// <summary>
        /// Called by animation event for executing charge attacks (e.g. rushing forward).
        /// </summary>
        public void PerformChargeAttackEvent()
        {
            if (bossDragonAI != null)
            {
                bossDragonAI.PerformChargeAttack();
            }
            else
            {
                Debug.LogWarning("⚠️ DragonVisuals: Missing BossDragonAI reference for PerformChargeAttackEvent.");
            }
        }



        /// <summary>
        /// (Optional) Can add future event functions for footsteps, impact, etc.
        /// </summary>
        // public void FootstepEvent() { SoundManager.PlaySound("DragonFootstep"); }
    }
}
