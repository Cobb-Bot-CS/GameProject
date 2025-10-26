using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            
            audioSource.PlayOneShot(jumpSound);
        }
    }
}
