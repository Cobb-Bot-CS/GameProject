#if UNITY_EDITOR
using log4net.Util;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMove : MonoBehaviour
{
//--------------------Character Components & Constraints-------------------------//
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveSpeed = 5f;
    private float jumpStrength = 9f;
    private float jumpLimiter = .1f;
    public Joystick joystick;

//--------------------Character Move Action Variables-------------------------//
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

//--------------------Character Attack Action Variables-------------------------//
    [SerializeField] private CharacterAttack attackScript;
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    private float cooldownTime = 1f;
    private float nextClickTime = 0f;

//--------------------Character Animation Variables-------------------------//
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;


//--------------------Character Footstep Timing-------------------------//
    [SerializeField] private float footstepInterval = 0.35f;
    private float nextFootstepTime = 0f;

//--------------------Enabling New Unity Movement Via Keyboard Input-------------------------//
    void OnEnable()
    {
    //Produce a continuous value by using the bounded buttons A and D for Left - Right//
        moveAction = new InputAction(type: InputActionType.Value);
        //moveAction.AddBinding("<Keyboard>/d");
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");

    //Produce a jump action, bound to the space bar//
        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");

    //Produce an attack action, bound to the left click
    attackAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
    attackAction.performed += PerformAttack;
    

    //Enable both move and jump actions when function is called//
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }

//--------------------Disabling Movement (For Cooldowns, etc.)-------------------------//
    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();

    if (attackAction != null)
    {
        attackAction.performed -= PerformAttack; // unsubscribe properly
        attackAction.Disable();
    }
    }

    void Start()
    {
    //Grab Components of the Character for Movement and Animations//
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

//--------------------Movement Check Every Frame via Update Function-------------------------//
    void Update()
    {
        float moveX = 0f;

    //Check for Joystick / Keyboard and Applies Vector Movement Based on Above Keybinds//
        if (joystick != null)
        {
            moveX = joystick.Horizontal();
            if (Mathf.Abs(moveX) < 0.1f) moveX = moveAction.ReadValue<float>();
        }
        else moveX = moveAction.ReadValue<float>();

        movement = new Vector2(moveX, rb.linearVelocity.y);

    //Apply sound(s) when jump is triggered and prevents double jumping via Jump Limiter//
        if (jumpAction.triggered && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            AudioManager.Instance.Play("Jump");
        }

    //Same as Above, but for Mobile Controls//
        if (joystick != null && joystick.Vertical() > 0.6f && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            AudioManager.Instance.Play("Jump");
        }

    //Flip Character Sprite & Hitbox when Character Faces Opposing Directions//
        if (moveX != 0)
        {
            bool facingLeft = moveX > 0;
            spriteRenderer.flipX = facingLeft;

            Vector2 offset = weaponHitbox.offset;
            offset.x = Mathf.Abs(offset.x) * (facingLeft ? 1 : -1);
            weaponHitbox.offset = offset;
        }

    //Animate the Character's Walking by calling on the animator bools//
        animator.SetBool("IsWalking", moveX != 0);

    //Set Logic for Time Between Footsteps & Footstep Sounds//
        if (moveX != 0 && Mathf.Abs(rb.linearVelocity.y) < jumpLimiter)
        {
            if (Time.time >= nextFootstepTime)
            {
                AudioManager.Instance.Play("Footstep");
                nextFootstepTime = Time.time + footstepInterval;
            }
        }

    //Logic for Playing Sounds On Character Attack//

       /* if (Input.GetMouseButtonDown(0) && Time.time >= nextClickTime)
        {
            AudioManager.Instance.Play("SwordAttack");
            StartCoroutine(attackScript.Attack());
            nextClickTime = Time.time + cooldownTime;
        }*/
    }

//--------------------Smoother Movement Through FixedUpdate Function-------------------------//
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    //Access Animator Bool To Play Hurt Animation, and Cooldown On Walking/Jumping//
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

//--------------------Mobile Device/Regular Attacking Functions-------------------------//

private void PerformAttack(InputAction.CallbackContext ctx)
{
    if (this == null) return; // guard against destroyed object
    if (attackScript == null) return; // guard against missing reference

    if (Time.time >= nextClickTime)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.Play("SwordAttack");

        StartCoroutine(attackScript.Attack());
        nextClickTime = Time.time + cooldownTime;
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

//--------------------TESTING-------------------------//

//Increasing Movement Speed For Unity Testing Purposes//
    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed = amount;
    }
}
