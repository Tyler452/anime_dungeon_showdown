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

    [Header("Status Effects")]
    private bool burning = false;
    private float originalSpeed;

    void Start()
    {
        // set starting health
        currentHealth = maxHealth;

        // store base speed in case we apply slow
        originalSpeed = moveSpeed;

        // find the player in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // chase player if inside detection range
        if (distance <= chaseRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            transform.position += direction * moveSpeed * Time.deltaTime;

            transform.LookAt(player);
        }

        // attack if close enough
        if (distance <= attackRange && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // attack logic would go here (damage player later)

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyBurn(float damage, float duration)
    {
        if (!burning)
        {
            StartCoroutine(Burn(damage, duration));
        }
    }

    IEnumerator Burn(float damage, float duration)
    {
        burning = true;

        float timer = 0;

        while (timer < duration)
        {
            timer += 1f;
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
        Destroy(gameObject);
    }
}