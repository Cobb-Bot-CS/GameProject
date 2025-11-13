using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // 虚函数：允许子类重写
    public virtual float GetMeleeDamage()
    {
        Debug.Log("[EnemyBase] Default melee damage");
        return 10f;
    }
}
