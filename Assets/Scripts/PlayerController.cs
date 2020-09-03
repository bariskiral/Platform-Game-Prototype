using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputControls inputControls;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private RaycastHit2D ladderInfo;

    private int extraJumps;
    private bool isGrounded;
    private bool isClimbing;
    private bool isDashing;
    private float currDashTimer;
    private float elapsedTime = 0f;
    public static float playerDirection = 1f;

    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask whatIsLadder;
    [SerializeField] private Transform groudCheck;

    [SerializeField] private int jumpCount;
    [SerializeField] private float checkRadius;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashTimer;
    [SerializeField] private float dashDelay;

    private void Awake()
    {
        inputControls = new InputControls();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        extraJumps = jumpCount;
        inputControls.Player.Jump.performed += ctx => Jump();
        inputControls.Player.Dash.performed += ctx => Dash();
    }

    private void Update()
    {
        CheckInput();   
    }

    private void FixedUpdate()
    {
        Movement();
        LadderClimb();
        Flip();
        Dashing();
        ImprovedGravity();
        CheckSurroundings();
    }

    private void CheckInput()
    {
        moveInput = inputControls.Player.Move.ReadValue<Vector2>();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groudCheck.position, checkRadius, ground);
        ladderInfo = Physics2D.Raycast(transform.position, Vector2.up, 5, whatIsLadder);
    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Math.Abs(moveInput.x));
    }

    private void Flip()
    {
        if (moveInput.x < 0)
        {
            transform.eulerAngles = new Vector2(0, -180);
            playerDirection = -1;
        }
        else if (moveInput.x > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            playerDirection = 1;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            extraJumps = jumpCount;
        }

        if (extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpSpeed;
            extraJumps--;
        }
    }

    private void LadderClimb()
    {
        if (ladderInfo.collider != null)
        {
            if (moveInput.y != 0)
            {
                isClimbing = true;
            }
        }
        else
        {
            if (moveInput.x != 0)
            {
                isClimbing = false;
            }
        }

        if (isClimbing && ladderInfo.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, moveInput.y * moveSpeed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 2f;
        }
    }

    private void Dash()
    {
        if (Time.time >= elapsedTime)
        {
            anim.SetTrigger("Dash");
            isDashing = true;
            currDashTimer = dashTimer;
            rb.velocity = Vector2.zero;
            elapsedTime = Time.time + dashDelay;
        }
    }

    private void Dashing()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2(dashPower * playerDirection, 0.0f);
            currDashTimer -= Time.deltaTime;

            if (currDashTimer <= 0)
            {
                isDashing = false;
            }
        }
    }

    private void ImprovedGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMultiplier + 1) * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        inputControls.Enable();
    }

    private void OnDisable()
    {
        inputControls.Disable();
    }

}
