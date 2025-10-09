using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    void Awake()
    {
        currentHP = maxHP;              // 关键：启动时填满血
    }

    public void Damage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        Debug.Log($"[Health] {name} -{amount}, HP={currentHP}");
        if (currentHP == 0) OnDeath();
    }

    void OnDeath()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.gray;  // 临时死亡表现
        Debug.Log($"[Health] {name} died");
    }
}

