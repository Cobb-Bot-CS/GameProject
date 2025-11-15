using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBarAmount;
    [SerializeField] private CharacterHealth playerHealth;

    void Start()
    {
        healthBarAmount.maxValue = playerHealth.maxHealth;
        healthBarAmount.minValue = playerHealth.minHealth;

        // Subscribe to health changes
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    void UpdateHealthBar(int newHealth)
    {
        healthBarAmount.value = newHealth;
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }
}
