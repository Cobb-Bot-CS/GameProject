using System.Data.Common;
using UnityEngine;

public class CharacterHealthScript : MonoBehaviour
{
    //Health Related Variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int minHealth = 0;
    [SerializeField] private int currentHealth;
    private bool ishurt = false;

    //Movement While Hurt / Animation
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterMove movement;

    void Start()
    {
        currentHealth = 100;
    }

    void CharacterHurt()
    {
        animator.SetBool("IsHurt", true);
        movement.CharacterHurtCooldown();
    }
}
