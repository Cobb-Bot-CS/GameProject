
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.TextCore.Text;

public class CharacterMove : MonoBehaviour
{
    //Movement Variables
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSpeed = 5f;
    private float jumpStrength = 9f;
    private float jumpLimiter = .1f;

    //Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;

    //Attack Variables
    [SerializeField] private CharacterAttack attackScript;
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    private float cooldownTime = 1f;
    private float nextClickTime = 0f;

    //Animator Variables
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        //Defining Input Actions
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
        //Getting Components Upon Startup
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Input Manager Using New Unity Input System
        float moveX = moveAction.ReadValue<float>();
        movement = new Vector2(moveX, rb.linearVelocity.y);

        if (jumpAction.triggered && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
        }

        //Sprite Flipper Depending On Direction
        if (moveX != 0)
        {
            bool facingLeft = moveX > 0;
            spriteRenderer.flipX = facingLeft;

            Vector2 offset = weaponHitbox.offset;
            offset.x = Mathf.Abs(offset.x) * (facingLeft ? 1 : -1);
            weaponHitbox.offset = offset;
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
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
            Debug.Log("Time is up to " + nextClickTime);
        }
    }

    void FixedUpdate()
    {
        //Allows For Smooth Horizontal Movement 
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
            {
                
            }
        }

    }

    //BOUNDS TEST ONLY TEMPORARY
    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed = amount;
    }
}
