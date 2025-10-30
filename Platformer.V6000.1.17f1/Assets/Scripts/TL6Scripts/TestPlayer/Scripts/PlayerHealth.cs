using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设定")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("UI组件")]
    [SerializeField] private Slider healthBar;

    [Header("反馈效果")]
    [SerializeField] private GameObject hurtEffect;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private GameObject blockSuccessEffect; // <<< NEW
    [SerializeField] private AudioClip blockSuccessSound;  // <<< NEW
    [SerializeField] private float invincibilityDuration = 1f;
    private bool isInvincible = false;

    // 缓存组件引用
    private Animator animator;
    private PlayerController playerController; // 确保有这个引用
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>(); // 获取引用
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        // --- 核心格挡逻辑 --- // <<< NEW
        if (playerController != null && playerController.IsBlocking)
        {
            if (Random.value <= playerController.GetBlockChance())
            {
                Debug.Log("格挡成功！");
                if (blockSuccessEffect != null) Instantiate(blockSuccessEffect, transform.position, Quaternion.identity);
                if (blockSuccessSound != null) audioSource.PlayOneShot(blockSuccessSound);
                return; // 免疫伤害，提前结束函数
            }
            else
            {
                Debug.Log("格挡失败！");
            }
        }
        // --- 格挡逻辑结束 ---

        // 正常受伤流程
        currentHealth -= damage;
        if (healthBar != null) healthBar.value = currentHealth;

        if (hurtSound != null) audioSource.PlayOneShot(hurtSound);
        if (hurtEffect != null) Instantiate(hurtEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private void Die()
    {
        Debug.Log("player dead!");
        if (playerController != null) playerController.enabled = false;
        if (animator != null) animator.SetTrigger("Death");
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        for (float i = 0; i < invincibilityDuration; i += 0.15f)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.15f);
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}