using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private string pickupSoundName = "SwordPickup";
    
    [Header("Optional: Fallback Sound")]
    [Tooltip("Only used if AudioManager is not found")]
    [SerializeField] private AudioClip fallbackSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayPickupSound();
            Destroy(gameObject);
        }
    }

    private void PlayPickupSound()
    {
        // Try to use AudioManager first
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(pickupSoundName);
        }
        // Fallback to direct playback if AudioManager isn't available
        else if (fallbackSound != null)
        {
            AudioSource.PlayClipAtPoint(fallbackSound, transform.position);
            Debug.LogWarning("AudioManager not found - using fallback sound");
        }
        else
        {
            Debug.LogWarning("No audio playback available for sword pickup");
        }
    }
}