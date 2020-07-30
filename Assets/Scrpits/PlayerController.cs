using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private InputControls inputControls;
    private Rigidbody2D rb;
    private Collider2D col;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private LayerMask ground;

    private void Awake()
    {
        inputControls = new InputControls();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
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
        inputControls.Player.Jump.performed += ctx => Jump();
        inputControls.Player.Fire.performed += ctx => Attack();
    }

    private void Attack()
    {
        Debug.Log("Fire!");
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Vector2 topLeftPoint = transform.position;
        topLeftPoint.x -= col.bounds.extents.x;
        topLeftPoint.y += col.bounds.extents.y;

        Vector2 bottomLeftPoint = transform.position;
        bottomLeftPoint.x += col.bounds.extents.x;
        bottomLeftPoint.y -= col.bounds.extents.y;

        return Physics2D.OverlapArea(topLeftPoint, bottomLeftPoint, ground);
    }


    void Update()
    {
        float moveInput = inputControls.Player.Move.ReadValue<float>();
        rb.velocity = new Vector2(moveInput*moveSpeed, rb.velocity.y);
        //if (rb.velocity.y < 0)
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //}

    }

}
