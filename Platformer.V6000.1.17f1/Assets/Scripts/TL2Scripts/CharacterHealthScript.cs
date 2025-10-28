using System.Data.Common;
using UnityEngine;

public class CharacterHealthScript : MonoBehaviour
{
    //Health Related Variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int minHealth = 0;
    public int currentHealth = 100;

    //Movement While Hurt / Animation
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMove movement;

    void Start()
    {
        currentHealth = 100;
    }

    public void CharacterHurt(int amount)
    {
        //Input Sanitization
        if (amount < 0) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        //Animates Damage Taken
        animator.SetBool("IsHurt", true);
        StartCoroutine(movement.CharacterHurtCooldown());

        if (currentHealth <= minHealth)
        {
            Die();
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    private void Die()
    {
            animator.SetBool("IsHurt", true);
            Debug.Log("Character Died");
            //trigger game over screen]
            //restart game
    }

    private void NoOverhealing()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = 100;
        }
    }
    
}
