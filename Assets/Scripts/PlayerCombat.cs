using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private InputControls inputControls;
    private GameObject player;
    private PlayerHealth playerHealth;

    private float elapsedTime = 0;

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private bool rangedWeapon = true;
    [SerializeField] private bool meleeWeapon = false;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float shotForce = 25f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float swordDamage = 1f;

    private void Awake()
    {
        inputControls = new InputControls();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Start()
    {
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
        if (rangedWeapon && Time.time >= elapsedTime)
        {
            BowAttack();
            elapsedTime = Time.time + attackDelay;
        }
        if (meleeWeapon && Time.time >= elapsedTime)
        {
            SwordAttack();
            elapsedTime = Time.time + attackDelay;
        }
    }

    private void BowAttack()
    {
        anim.SetTrigger("Fire");

        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.right * shotForce;
    }

    private void SwordAttack()
    {
        anim.SetTrigger("Hit");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyScript>().EnemyTakeDamage(swordDamage);
            playerHealth.GainHealth(swordDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
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
