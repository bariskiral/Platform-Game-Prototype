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
    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;
    private BowScript bow;

    private bool isGrounded;
    private int extraJumps;
    private float elapsedTime = 0;

    [SerializeField] private Animator anim;
    [SerializeField] private Transform groudCheck;
    [SerializeField] private LayerMask ground;

    [SerializeField] private int jumpCount;
    [SerializeField] private float checkRadius;
    [SerializeField] private float dashPower = 100f;
    [SerializeField] private float dashCD;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private bool rangedWeapon = true;
    [SerializeField] private bool meleeWeapon = false;



    private void Awake()
    {
        inputControls = new InputControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        bow = GetComponentInChildren<BowScript>();
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
        inputControls.Player.Dash.performed += ctx => Dash();
        inputControls.Player.Attack.performed += ctx => Attack();
        inputControls.Player.WeaponSwap.performed += ctx => Swap();
    }

    private void Swap()
    {
        if (rangedWeapon)
        {
            meleeWeapon = true;
            rangedWeapon = false;
        }
        else
        {
            meleeWeapon = false;
            rangedWeapon = true;
        }
    }

    private void Attack()
    {
        //TODO
        if (rangedWeapon && Time.time > elapsedTime)
        {
            anim.SetTrigger("Fire");
            bow.GetComponent<BowScript>().BowAttack();
            elapsedTime = Time.time + attackDelay;
        }
        if (meleeWeapon && Time.time > elapsedTime)
        {
            anim.SetTrigger("Hit");
            
            elapsedTime = Time.time + attackDelay;
        }
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
        #region Movement
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
        #endregion
    }

}
