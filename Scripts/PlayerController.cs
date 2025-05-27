using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;

    [Header("Projectile Settings")]
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public int maxProjectile;
    private int currentProjectile;

    [Header("Melee Attack Settings")]
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayer;
    public int meleeDamage;
    public float attackOffset;

    [Header("Health Settings")]
    public int maxHealth;
    private int currentHealth;

    [Header("UI Settings")]
    public Text projectileText;
    public Image healthBarFill;
    
    private Rigidbody2D rig;
    private Animator anim;
    private Vector2 lastDirection = Vector2.down;
    private bool isWalk = false;
    private bool isShooting = false;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentProjectile = maxProjectile;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Attack();
        Shooting();

        projectileText.text = currentProjectile.ToString();

        if(attackPoint != null)
        {
            attackPoint.localPosition = lastDirection * attackOffset;
        }
    }

    public void Walk()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(x, y);

        rig.velocity = moveDir * speed;

        isWalk = moveDir != Vector2.zero;

        if (isWalk)
        {
            lastDirection = moveDir.normalized;

            //shootPoint.localPosition = lastDirection;
        }

        anim.SetFloat("axisX", lastDirection.x);
        anim.SetFloat("axisY", lastDirection.y);
        anim.SetBool("walk", isWalk);
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("attack");
        }
    }

    public void Shooting()
    {
        if (Input.GetButtonDown("Fire2") && !isShooting)
        {
            isShooting = true;
            rig.velocity = Vector2.zero;
            anim.SetTrigger("shoot");
        }
    }

    public void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            if(enemyController != null)
            {
                enemyController.TakeDamage(meleeDamage);
            }
        }
    }

    public void ProjectileFire()
    {
        Debug.Log("Disparou flecha");

        if(currentProjectile <= 0)
        {
            return;
        }

        if (lastDirection == Vector2.zero)
        {
            lastDirection = Vector2.down;
        }

        currentProjectile--;

        Vector2 spawnOffset = lastDirection.normalized * 0.5f;
        Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        projectile.GetComponent<Rigidbody2D>().velocity = lastDirection * projectileSpeed;

        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public void EndShoot()
    {
        isShooting = false;
    }

    public void AddProjectile(int amount)
    {
        currentProjectile = Mathf.Clamp(currentProjectile + amount, 0, maxProjectile);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if(currentHealth <= 0)
        {
            Death();
        }
    }

    private void UpdateHealthBar()
    {
        if(healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void Death()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
