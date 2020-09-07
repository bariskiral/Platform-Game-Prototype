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
    private RaycastHit2D wallInfo;

    private int extraJumps;
    private float currDashTimer;
    private float elapsedTime = 0f;
    public static float playerDirection = 1f;

    [Header("Drag Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask whatIsLadder;
    [SerializeField] private Transform groudCheck;
    [SerializeField] private Transform wallCheck;

    [Header("Behaviours")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool isClimbing;
    [SerializeField] private bool isWallSliding;

    [Header("Movement")]
    [SerializeField] private float checkRadius;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rbGravityScale = 2f;
    [SerializeField] private float wallCheckDis;
    [SerializeField] private float wallSlideSpeed;

    [Header("Jump")]
    [SerializeField] private int jumpCount;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float jumpDelay;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Dash")]
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
        CheckWallSliding();
    }

    private void CheckInput()
    {
        moveInput = inputControls.Player.Move.ReadValue<Vector2>();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groudCheck.position, checkRadius, ground);
        ladderInfo = Physics2D.Raycast(transform.position, Vector2.up, 2, whatIsLadder);
        wallInfo = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDis, ground);

        Debug.DrawRay(wallCheck.position, new Vector3(wallCheckDis * playerDirection, 0, 0), Color.white);
    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Math.Abs(moveInput.x));
        
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
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
        if (Time.time >= elapsedTime)
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

            elapsedTime = Time.time + jumpDelay;
        }
    }

    private void CheckWallSliding()
    {
        if (wallInfo && Math.Round(moveInput.x) == playerDirection && rb.velocity.y < 0 && !isClimbing)
        {
            isWallSliding = true;
            extraJumps = jumpCount;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void LadderClimb()
    {
        if (ladderInfo.collider != null )
        {
            isClimbing = true;
        }
        else if(moveInput.x != 0)
        {
            isClimbing = false;
        }

        if (isClimbing && ladderInfo.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, moveInput.y * moveSpeed);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = rbGravityScale;
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
