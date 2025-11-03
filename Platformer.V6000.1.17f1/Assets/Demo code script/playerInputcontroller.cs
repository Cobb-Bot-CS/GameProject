using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundMask;

    private bool inputEnabled = true;

    void Update()
    {
        // If input disabled (demo mode active), stop checking for player input
        if (!inputEnabled) return;

        // Move left / right
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // Jump if on ground
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundMask);
        if (onGround && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void EnableInput(bool enable)
    {
        inputEnabled = enable;
        if (!enable)
        {
            // Stop player motion when disabling input
            rb.linearVelocity = Vector2.zero;
        }
    }
}
