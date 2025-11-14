using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    //--------------------Health-Related Variables-------------------------//
    public int maxHealth = 100;
    public int minHealth = 0;
    public int currentHealth = 100;

    //--------------------Win-Death Screen References-------------------------// 
    [SerializeField] private GameObject deathScreen; 
    [SerializeField] private GameObject winScreen;

    //--------------------Events for Observers-------------------------//
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnWin;
    public event Action<int> OnMaxHealthChanged;
    public event Action<int> OnMinHealthChanged;

    //--------------------Animator References-------------------------// 
    [SerializeField] private Animator animator; 
    [SerializeField] private CharacterMove movement;

    void Start()
    {
        currentHealth = maxHealth;

        // Notify observers of initial values
        OnHealthChanged?.Invoke(currentHealth);
        OnMaxHealthChanged?.Invoke(maxHealth);
        OnMinHealthChanged?.Invoke(minHealth);
    }

    //--------------------Function for Character Damage-------------------------//
    public void CharacterHurt(int amount)
    {
        //Input Sanitization// 
        if (amount < 0) return; 
        currentHealth -= amount; 
        currentHealth = Mathf.Max(currentHealth, minHealth); 
        OnHealthChanged?.Invoke(currentHealth);

        //Animates When Damage Taken//
        animator.SetBool("IsHurt", true); 

        //Movement Cooldown//
        StartCoroutine(movement.CharacterHurtCooldown()); 

        if (currentHealth <= minHealth) 
        { 
            Die(); 
        } 
    }

    //Checking Function for Character's Current Health// 
    public int GetHealth() 
    { 
        return currentHealth; 
    }


    //--------------------Character Death Function-------------------------//
    private void Die()
    {
        Debug.Log("Character Died");

        //Stop player movement//
        movement.enabled = false;

        //Pause Game Time//
        Time.timeScale = 0f;

        //Show Death Screen 
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
        OnDeath?.Invoke();
    }

    //--------------------Character Win Function-------------------------//
    public void Win()
    {
        Debug.Log("You Win!");

        //Stop player movement 
        movement.enabled = false;

        //Pause Game Time 
        Time.timeScale = 0f;

        //Show Win Screen 
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        OnWin?.Invoke();
    }

    //--------------------Prevent Over-Healing-------------------------//
    public void NoOverhealing()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    //--------------------Restart Game Function-------------------------//
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


