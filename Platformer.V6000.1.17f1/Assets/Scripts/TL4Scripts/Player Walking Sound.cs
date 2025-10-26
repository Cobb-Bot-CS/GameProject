using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private float stepInterval = 0.4f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float stepTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isMoving && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                audioSource.PlayOneShot(walkSound);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}
