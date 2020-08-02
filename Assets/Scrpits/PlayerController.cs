using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private InputControls inputControls;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;

    private bool isGrounded;
    private int extraJumps;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform groudCheck;
    [SerializeField] private LayerMask ground;

    [SerializeField] private int jumpCount;
    [SerializeField] private float checkRadius;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpSpeed = 15f;



    private void Awake()
    {
        inputControls = new InputControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        inputControls.Enable();
    }

    private void OnDisable()
    {
        inputControls.Disable();    
    }

    void Start()
    {
        extraJumps = jumpCount;
        inputControls.Player.Jump.performed += ctx => Jump();
        inputControls.Player.Attack.performed += ctx => Attack();
        inputControls.Player.Dash.performed += ctx => Dash();
    }

    private void Attack()
    {
        anim.SetTrigger("Fire");
        playerHealth.gainHealth(1);
    }

    private void Dash()
    {
        anim.SetTrigger("Dash");
        //TODO
    }

    private void Jump()
    {
        if (isGrounded)
        {
            extraJumps = jumpCount;
        }

        if (extraJumps>0)
        {
            rb.velocity = Vector2.up * jumpSpeed;
            extraJumps--;
        }
    }

    void FixedUpdate()
    {
        //Are we collide with ground?
        isGrounded = Physics2D.OverlapCircle(groudCheck.position, checkRadius, ground);

        //Sideways movement
        Vector2 moveInput = inputControls.Player.Move.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveInput.x*moveSpeed, rb.velocity.y);

        anim.SetFloat("Speed", Math.Abs(moveInput.x));

        //Sprite flip
        if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }


        //Gravity changer for speed up jumping and falling
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMultiplier + 1) * Time.deltaTime;
        }

    }
}
