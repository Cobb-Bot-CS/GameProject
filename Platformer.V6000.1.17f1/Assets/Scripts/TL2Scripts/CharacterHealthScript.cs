using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealthScript : MonoBehaviour
{
    //Health Related Variables
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth = 100;

    //Movement While Hurt / Animation
    [SerializeField] private GameObject deathScreen; 
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMove movement;

    void Start()
    {
        currentHealth = maxHealth;
        if (deathScreen != null)
            deathScreen.SetActive(false);
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
        Debug.Log("Character Died");
        animator.SetBool("IsHurt", true);
        movement.enabled = false; // stop player movement
        Time.timeScale = 0f; // pause game

        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    private void NoOverhealing()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = 100;
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // unpause game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}
