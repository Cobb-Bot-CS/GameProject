#if UNITY_EDITOR
using log4net.Util;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSpeed = 5f;
    private float jumpStrength = 9f;
    private float jumpLimiter = .1f;
    public Joystick joystick;

    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField] private CharacterAttack attackScript;
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    private float cooldownTime = 1f;
    private float nextClickTime = 0f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //  Footstep timing
    [SerializeField] private float footstepInterval = 0.35f;
    private float nextFootstepTime = 0f;

    void OnEnable()
    {
        moveAction = new InputAction(type: InputActionType.Value, binding: "<Keyboard>/a");
        moveAction.AddBinding("<Keyboard>/d");
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
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = 0f;

        //  PC + Mobile movement input support
        if (joystick != null)
        {
            moveX = joystick.Horizontal();
            if (Mathf.Abs(moveX) < 0.1f) moveX = moveAction.ReadValue<float>();
        }
        else moveX = moveAction.ReadValue<float>();

        movement = new Vector2(moveX, rb.linearVelocity.y);

        // Jump sound
        if (jumpAction.triggered && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            AudioManager.Instance.Play("Jump");
        }

        if (joystick != null && joystick.Vertical() > 0.6f && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            AudioManager.Instance.Play("Jump");
        }

        //  Facing direction + weapon flip
        if (moveX != 0)
        {
            bool facingLeft = moveX > 0;
            spriteRenderer.flipX = facingLeft;

            Vector2 offset = weaponHitbox.offset;
            offset.x = Mathf.Abs(offset.x) * (facingLeft ? 1 : -1);
            weaponHitbox.offset = offset;
        }

        animator.SetBool("IsWalking", moveX != 0);

        // FOOTSTEP LOOPING LOGIC
        if (moveX != 0 && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            if (Time.time >= nextFootstepTime)
            {
                AudioManager.Instance.Play("Footstep");
                nextFootstepTime = Time.time + footstepInterval;
            }
        }

        // ✅ Attack + sound
        if (Input.GetMouseButtonDown(0) && Time.time >= nextClickTime)
        {
            AudioManager.Instance.Play("SwordAttack");
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    // ✅ Hurt + sound
    public IEnumerator CharacterHurtCooldown()
    {
        if (animator.GetBool("IsHurt") == true)
        {
        
            AudioManager.Instance.Play("PlayerHurt");
        
            moveAction.Disable();
            jumpAction.Disable();

            yield return new WaitForSeconds(1f);

            animator.SetBool("IsHurt", false);
            moveAction.Enable();
            jumpAction.Enable();
        }
    }

    public void MobileAttack()
    {
        if (Time.time >= nextClickTime)
        {
            AudioManager.Instance.Play("SwordAttack");
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
        }
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed = amount;
    }
}
