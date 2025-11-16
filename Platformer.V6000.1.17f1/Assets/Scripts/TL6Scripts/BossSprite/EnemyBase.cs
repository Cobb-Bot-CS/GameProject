using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public virtual float GetMeleeDamage()
    {
        Debug.Log("[EnemyBase] Default melee damage");
        return 10f;
    }
}

