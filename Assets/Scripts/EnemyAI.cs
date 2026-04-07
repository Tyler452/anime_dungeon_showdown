using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaseRange = 15f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private bool canAttack = true;

    [Header("Hit Feedback")]
    public GameObject hitVFX;
    public AudioClip hitSFX;
    private AudioSource audioSource;

    [Header("Status Effects")]
    private bool burning = false;
    private float originalSpeed;

    void Start()
    {
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.LookAt(player);
        }

        if (distance <= attackRange && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }

        if (hitSFX != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyBurn(float damage, float duration)
    {
        if (!burning)
            StartCoroutine(Burn(damage, duration));
    }

    IEnumerator Burn(float damage, float duration)
    {
        burning = true;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            TakeDamage(damage);
            yield return new WaitForSeconds(1f);
        }

        burning = false;
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        StartCoroutine(Slow(slowAmount, duration));
    }

    IEnumerator Slow(float slowAmount, float duration)
    {
        moveSpeed *= slowAmount;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }

    void Die()
    {
        FindObjectOfType<EnemySpawner>()?.NotifyEnemyDied();
        Destroy(gameObject);
    }
}