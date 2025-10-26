using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    public AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject); // Remove sword from scene
        }
    }
}
