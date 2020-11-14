using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputControls inputControls;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private RaycastHit2D ladderInfo;
    private RaycastHit2D wallInfo;
    private RaycastHit2D ledgeInfo;
    private Vector2 playerCoord;
    private Vector2 offsetCoord;
    private TrailRenderer trailRenderer;

    private int extraJumps;
    private float currDashTimer;
    private float hangCounter;
    private float elapsedTime = 0f;
    private float playerDirection = 1f;

    [Header("Drag Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsLadder;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsLedge;
    [SerializeField] private Transform groudCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;

    [Header("Behaviours")]
    [SerializeField] private bool isGrounded;
    [SerializeField] public bool isDashing;
    [SerializeField] private bool isClimbing;
    [SerializeField] private bool isWallSliding;

    [Header("Movement")]
    [SerializeField] private float checkRadius;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rbGravityScale = 2f;
    [SerializeField] private float rayCheckDis;
    [SerializeField] private float wallSlideSpeed;

    [Header("Jump")]
    [SerializeField] private int jumpCount;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float jumpDelay;
    [SerializeField] private float hangTime = 0.15f;
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
        trailRenderer = GetComponent<TrailRenderer>();
        
    }

    void Start()
    {
        extraJumps = jumpCount;
        inputControls.Player.Jump.performed += ctx => HoldJump();
        inputControls.Player.Jump.canceled += ctx => TapJump();
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
        CheckLedgeClimb();
    }

    private void CheckInput()
    {
        moveInput = inputControls.Player.Move.ReadValue<Vector2>();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groudCheck.position, checkRadius, whatIsGround);
        ladderInfo = Physics2D.Raycast(transform.position, Vector2.up, 2, whatIsLadder);
        wallInfo = Physics2D.Raycast(wallCheck.position, transform.right, rayCheckDis, whatIsWall);
        ledgeInfo = Physics2D.Raycast(ledgeCheck.position, transform.right, rayCheckDis, whatIsLedge);

        Debug.DrawRay(wallCheck.position, new Vector3(rayCheckDis * playerDirection, 0, 0), Color.white);
        Debug.DrawRay(ledgeCheck.position, new Vector3(rayCheckDis * playerDirection, 0, 0), Color.white);
    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Math.Abs(moveInput.x));
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isGrounded", isGrounded);

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

        if (isGrounded)
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime; 
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

    private void HoldJump()
    {
        if (Time.time >= elapsedTime)
        {
            if (hangCounter > 0)
            {
                extraJumps = jumpCount;
            }

            if (extraJumps > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                anim.SetTrigger("Jump");
                extraJumps--;
            }

            elapsedTime = Time.time + jumpDelay;
        }
    }

    private void TapJump()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.05f);
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

    private void CheckLedgeClimb()
    {
        if (wallInfo && !ledgeInfo && !isClimbing)
        {
            anim.SetTrigger("Climb");
            playerCoord = transform.position;
            offsetCoord = new Vector2(1f * playerDirection, 1f);
        }       
    }
    private void CommitClimb()
    {
        transform.position = playerCoord + offsetCoord;
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
        Physics2D.IgnoreLayerCollision(9, 11, isDashing);
        Physics2D.IgnoreLayerCollision(9, 14, isDashing);
        trailRenderer.enabled = isDashing;

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
