using UnityEngine;
/*
 * Filename: EnemyBase.cs
 * Developer: Qiwei Liang
 * Purpose: This file is for dynamic binding specially
 */

public class EnemyBase : MonoBehaviour
{
    public virtual float GetMeleeDamage()
    {
        Debug.Log("[EnemyBase] Default melee damage");
        return 10f;
    }
}

