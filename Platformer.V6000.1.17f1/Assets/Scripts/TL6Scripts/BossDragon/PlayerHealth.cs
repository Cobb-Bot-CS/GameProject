using UnityEngine;
using UnityEngine.UI; // 如果有UI血条
namespace BossOne
{

    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth = 100f;
        private float currentHealth;

        public Slider healthSlider; // 可选：用于显示血条

        void Start()
        {
            currentHealth = maxHealth;
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        // !!! 确保有这个公共方法 !!!
        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0); // 确保血量不低于0

            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

            Debug.Log($"玩家受到 {damage} 伤害，剩余血量：{currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("玩家死亡！");
            // 可以在这里添加玩家死亡的逻辑
            gameObject.SetActive(false); // 简单地禁用玩家对象
        }
    }
}