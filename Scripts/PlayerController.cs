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

    [Header("Armor Settings")]
    public float armorDuration;
    public Image armorUI;
    public bool isInvincible = false;

    [Header("SpeedBoost Settings")]
    private float initialSpeed;
    private Coroutine speedBoostRoutine;
    public Image speedBoostUI;

    [Header("UI Settings")]
    public Text projectileText;
    public Image healthBarFill;
    public GameObject deathPanel;

    [Header("Audios Settings")]
    public AudioSource attackMeleeSound;
    public AudioSource footstepSound;
    public AudioSource shootSound;

    [Header("Damage Feedback")]
    public float damageColorDuration;
    private Color damageColor = Color.red;
    
    private Rigidbody2D rig;
    private Animator anim;
    private Vector2 lastDirection = Vector2.down;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isWalk = false;
    private bool isShooting = false;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentProjectile = maxProjectile;
        currentHealth = maxHealth;
        initialSpeed = speed;
        UpdateHealthBar();

        if(deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }

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
        if (isDead) { return; }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(x, y);

        rig.velocity = moveDir * speed;

        isWalk = moveDir != Vector2.zero;

        if (isWalk)
        {
            lastDirection = moveDir.normalized;

            if (!footstepSound.isPlaying)
            {
                footstepSound.Play();
            }
        }
        else
        {
            if (footstepSound.isPlaying)
            {
                footstepSound.Stop();
            }
        }

        anim.SetFloat("axisX", lastDirection.x);
        anim.SetFloat("axisY", lastDirection.y);
        anim.SetBool("walk", isWalk);
    }

    public void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            attackMeleeSound.Play();
            anim.SetTrigger("attack");
        }
    }

    public void Shooting()
    {
        if (Input.GetButtonDown("Fire2") && !isShooting)
        {
            isShooting = true;
            rig.velocity = Vector2.zero;
            shootSound.Play();
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
        if (isInvincible || isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if(currentHealth <= 0)
        {
            Death();
        }
        else
        {
            StartCoroutine(FlashDamageColor());
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
        isDead = true;
        rig.velocity = Vector2.zero;
        anim.SetTrigger("death");

        GetComponent<Collider2D>().enabled = false;

        footstepSound.Stop();

        StartCoroutine(ShowDeathPanelWithDelay(1.5f));
    }

    public void ActiveArmor(float duration)
    {
        if (isInvincible)
        {
            return;
        }

        StartCoroutine(ArmorRoutine(duration));
    }

    IEnumerator ArmorRoutine(float duration)
    {
        isInvincible = true;

        if(armorUI != null)
        {
            armorUI.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(duration);

        isInvincible = false;

        if(armorUI != null)
        {
            armorUI.gameObject.SetActive(false);
        }
    }

    public void ActiveSpeedBoost(float boostedSpeed, float duration)
    {
        if(speedBoostRoutine != null)
        {
            StopCoroutine(speedBoostRoutine);
        }

        speedBoostRoutine = StartCoroutine(SpeedBoostRoutine(boostedSpeed, duration));
    }

    IEnumerator SpeedBoostRoutine(float boostedSpeed, float duration)
    {
        speed = boostedSpeed;

        if(speedBoostUI != null)
        {
            speedBoostUI.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        speed = initialSpeed;

        if(speedBoostUI != null)
        {
            speedBoostUI.enabled = false;
        }

        speedBoostRoutine = null;
    }

    IEnumerator ShowDeathPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Time.timeScale = 0f;

        if(deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
    }

    IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageColorDuration);
        spriteRenderer.color = originalColor;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnDrawGizmos()
    {
        if(attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
