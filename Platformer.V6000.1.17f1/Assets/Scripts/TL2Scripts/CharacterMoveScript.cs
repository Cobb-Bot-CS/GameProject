
using log4net.Util;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CharacterMove : MonoBehaviour
{
    // Movement variables
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSpeed = 5f;
    private float jumpStrength = 9f;
    private float jumpLimiter = .1f;      // Used instead of IsGrounded()
    public Joystick joystick;             // Mobile joystick reference

    // Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;

    // Attack variables
    [SerializeField] private CharacterAttack attackScript;
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    private float cooldownTime = 1f;
    private float nextClickTime = 0f;

    // Animator variables
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        // Define input actions (A/D for move, Space for jump)
        moveAction = new InputAction(type: InputActionType.Value, binding: "<Keyboard>/a");
        moveAction.AddBinding("Keyboard>/d");
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");

        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");

        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movement Input (Keyboard + Joystick)
        float moveX = 0f;

        if (joystick != null)
        {
            moveX = joystick.Horizontal();

            // Allow keyboard when joystick centered
            if (Mathf.Abs(moveX) < 0.1f)
            {
                moveX = moveAction.ReadValue<float>();
            }
        }
        else
        {
            moveX = moveAction.ReadValue<float>();
        }

        movement = new Vector2(moveX, rb.linearVelocity.y);

        // Jump Logic (velocity-based, no IsGrounded)

        // 1️⃣ Keyboard jump
        if (jumpAction.triggered && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        }

        // 2️⃣ Joystick upward drag jump
        if (joystick != null && joystick.Vertical() > 0.6f && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
        }

        // Sprite flipping depending on direction
        if (moveX != 0)
        {
            bool facingLeft = moveX > 0;
            spriteRenderer.flipX = facingLeft;

            Vector2 offset = weaponHitbox.offset;
            offset.x = Mathf.Abs(offset.x) * (facingLeft ? 1 : -1);
            weaponHitbox.offset = offset;
        }

        // Walking animation control
        animator.SetBool("IsWalking", moveX != 0);

        // PC Attack control (mouse left click)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextClickTime)
        {
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
            Debug.Log("Next click available at " + nextClickTime);
        }
    }

    void FixedUpdate()
    {
        // Smooth horizontal movement
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    public IEnumerator CharacterHurtCooldown()
    {
        if (animator.GetBool("IsHurt") == true)
        {
            moveAction.Disable();
            jumpAction.Disable();

            yield return new WaitForSeconds(1f);

            animator.SetBool("IsHurt", false);
            moveAction.Enable();
            jumpAction.Enable();
        }
    }

    // Mobile Attack trigger for UI button
    public void MobileAttack()
    {
        // Same logic as mouse click attack
        if (Time.time >= nextClickTime)
        {
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
            Debug.Log("Mobile attack triggered at " + Time.time);
        }
    }

    // Temporary function for testing
    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed = amount;
    }
}
