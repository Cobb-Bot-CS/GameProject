using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBarAmount;
    [SerializeField] private CharacterHealthScript playerHealth;

    void Start()
    {
        healthBarAmount.maxValue = playerHealth.maxHealth;
        healthBarAmount.minValue = playerHealth.minHealth;
    }

    void Update()
    {
        healthBarAmount.value = playerHealth.currentHealth; 
    }
}
