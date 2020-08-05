using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowScript : MonoBehaviour
{
    private InputControls inputControls;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float shotForce;
    [SerializeField] private Transform shotPoint;

    private void Awake()
    {
        inputControls = new InputControls();
    }

    void Update()
    {
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
            );
        transform.right = direction;
    }

    public void BowAttack()
    {
        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.right * shotForce;
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
