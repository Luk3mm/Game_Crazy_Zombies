using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public event System.Action OnDeath;

    public float speedEnemy;
    public int maxHealth;
    public float deathDelay;

    [Header("Enemy Attack Settings")]
    public int damageInPlayer;
    public float attackCoolDown;
    private float lastAttackTime;

    [Header("Drop Settings")]
    public GameObject arrowDropPrefab;
    [Range(0f, 100f)] public float dropChance;
    public float dropDistance;
    public float dropMoveDuration;

    [Header("Damage Feedback")]
    public float damageColorDuration;
    private Color damageColor = Color.red;

    private bool isDead = false;
    private int currentHealth;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private Rigidbody2D rig;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null || isDead)
        {
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rig.velocity = direction * speedEnemy;

        if (anim != null)
        {
            anim.SetFloat("axisX", direction.x);
            anim.SetFloat("axisY", direction.y);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            rig.velocity = Vector2.zero;
            Death();
        }
        else
        {
            StartCoroutine(FlashDamageColor());
        }
    }

    private void Death()
    {
        isDead = true;
        rig.velocity = Vector2.zero;

        if(anim != null)
        {
            anim.SetTrigger("death");
        }

        GetComponent<Collider2D>().enabled = false;

        DropArrow();

        OnDeath?.Invoke();

        Destroy(gameObject, deathDelay);
    }

    private void DropArrow()
    {
        if(arrowDropPrefab == null || Random.value > dropChance)
        {
            return;
        }

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = transform.position;

        GameObject drop = Instantiate(arrowDropPrefab, spawnPos, arrowDropPrefab.transform.rotation);

        StartCoroutine(MoveDrop(drop.transform, randomDir));
    }

    IEnumerator MoveDrop(Transform drop, Vector2 direction)
    {
        Vector3 start = drop.position;
        Vector3 end = start + (Vector3)(direction * dropDistance);

        float elapsed = 0f;

        while(elapsed < dropMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropMoveDuration;

            drop.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageColorDuration);
        spriteRenderer.color = originalColor;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Time.time > lastAttackTime + attackCoolDown)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if(player != null)
            {
                if(anim != null)
                {
                    anim.SetTrigger("attack");
                }

                player.TakeDamage(damageInPlayer);
                lastAttackTime = Time.time;
            }
        }
    }
}
