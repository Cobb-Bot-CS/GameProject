using System.Data.Common;
using UnityEngine;

public class CharacterHealthScript : MonoBehaviour
{
    //Health Related Variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int minHealth = 0;
    [SerializeField] private int currentHealth;

    //Movement While Hurt / Animation
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMove movement;

    void Start()
    {
        currentHealth = 100;
    }

    public void CharacterHurt(int amount)
    {
        animator.SetBool("IsHurt", true);
        StartCoroutine(movement.CharacterHurtCooldown());
        currentHealth = currentHealth - amount;
        Sanitization();
        Die();
    }

    private void Die()
    {
        if (currentHealth <= minHealth)
        {
            animator.SetBool("IsHurt", true);
            Debug.Log("Character Died");

            //trigger game over screen]
            //restart game
        }
    }

    private void NoOverhealing()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = 100;
        }
    }
    
    private void Sanitization()
    {
        if(currentHealth < 0 || currentHealth > 150)
        {
            currentHealth = 0;
        }
    }
}
