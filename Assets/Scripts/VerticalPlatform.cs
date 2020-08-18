using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private InputControls inputControls;
    private float waitTime;

    [SerializeField] private float holdTime = 0.1f;

    private void Awake()
    {
        inputControls = new InputControls();
    }

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    void FixedUpdate()
    {
        Vector2 moveVec = inputControls.Player.Move.ReadValue<Vector2>();

        if (moveVec.y >= 0)
        {
            waitTime = holdTime;
            effector.rotationalOffset = 0;
        }
        if (moveVec.y < 0)
        {
            if (waitTime <= 0)
            {
                effector.rotationalOffset = 180f;
                waitTime = holdTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    //Move with platform on collision

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        col.gameObject.transform.SetParent(null);
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
