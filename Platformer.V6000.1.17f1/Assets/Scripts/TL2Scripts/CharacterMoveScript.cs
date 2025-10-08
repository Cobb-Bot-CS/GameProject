using TMPro;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMove : MonoBehaviour
{
    //Movement Variables
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSpeed = 2f;
    private float jumpStrength = 5f;
    private float jumpLimiter = 0.1f;

    //Attack Variables
    private float cooldownTime = 1f;
    private float nextClickTime = 0f;

    //Animator Variables
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        //Getting Components Upon Startup
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        //Input Manager Using New Unity Input System
        float moveX = Input.GetAxis("Horizontal");
        movement = new Vector2(moveX, rb.linearVelocity.y);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
        }

        //Sprite Flipper Depending On Direction
        if (moveX != 0)
        {
            spriteRenderer.flipX = moveX > 0;
        }

        //Character Walking Animation Manager
        if (moveX != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        //Character Attacking Animation Manager
        if (Input.GetMouseButtonDown(0) && Time.time >= nextClickTime)
        {
            animator.SetBool("IsAttacking", true);
            //Insert Actual Attack Here Later
            nextClickTime = Time.time + cooldownTime;
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }

    }

    void FixedUpdate()
    {
        //Allows For Smooth Horizontal Movement 
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }
}
