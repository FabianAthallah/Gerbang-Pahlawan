using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D coll;
    
    private float dirX;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask jumpableGround;

    public enum MovementState{Idle, Running, Jumping, Falling}

    [SerializeField] private AudioSource JumpSoundEffect;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    // private void Update()
    // {
    //     dirX = Input.GetAxisRaw("Horizontal");

    //     rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

    //     if(playerCon.performed && IsGrounded())
    //     {
    //         JumpSoundEffect.Play();
    //         rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    //     }

    //     UpdateAnimationState();

    // }

    public void Move(){
        Vector2 moveDirection = InputManager.GetInstance().GetMoveDirection();
        dirX = moveDirection.x;
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    public void Jump()
    {
        bool jumpPressed = InputManager.GetInstance().GetJumpPressed();
        if(jumpPressed && IsGrounded())
        {
            JumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        Move();
        Jump();

        UpdateAnimationState();
    }

    public void UpdateAnimationState()
    {
        MovementState state;

        if(DialogueManager.GetInstance().dialogueIsPlaying){
            rb.velocity = Vector2.zero;
        }

        if(dirX > 0f)
        {
            state = MovementState.Running;
            sprite.flipX = false;
        }
        else if(dirX < 0f)
        {
            state = MovementState.Running;
            sprite.flipX = true;
        }
        else{
            state = MovementState.Idle;
        }

        if(rb.velocity.y > .1f)
        {
            state = MovementState.Jumping;
        }
        else if(rb.velocity.y < -.1f)
        {
            state = MovementState.Falling;
        }

        anim.SetInteger("State", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    // private void OnEnable()
    // {
    //     playerCon.Enable();
    // }

    // private void OnDisable()
    // {
    //     playerCon.Disable();
    // }
}
