using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private string footstepSFXName = "Walk"; // Must match name in AudioManager
    [SerializeField] private float stepInterval = 0.4f;

    [Header("Jump Settings")]
    [SerializeField] private string jumpSFXName = "Jump"; // Must match name in AudioManager
    [SerializeField] private float jumpCooldown = 0.3f; // Prevents double triggering

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private float stepTimer = 0f;
    private float lastJumpTime = -1f;
    private Rigidbody2D rb;
    private bool wasGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        // 🦶 Play footsteps when walking
        if (isMoving && isGrounded)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                AudioManager.Instance.PlaySFXWithRandomPitch(footstepSFXName, 0.95f, 1.05f);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        // 🦘 Play jump sound only once when leaving the ground
        if (wasGrounded && !isGrounded && rb.linearVelocity.y > 0.1f)
        {
            if (Time.time - lastJumpTime > jumpCooldown)
            {
                AudioManager.Instance.PlaySFX(jumpSFXName);
                lastJumpTime = Time.time;
            }
        }

        wasGrounded = isGrounded;
    }

    void OnDrawGizmosSelected()
    {
        // Debug view of ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
