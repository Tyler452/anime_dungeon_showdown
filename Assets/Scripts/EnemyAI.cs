using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaseRange = 30f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    private bool canAttack = true;

    [Header("Target")]
    public Transform target;

    private float originalSpeed;
    private bool burning = false;

    void Start()
    {
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;
    }

    void Update()
    {
        FindTarget();

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= chaseRange)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f;

            transform.position += dir * moveSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        if (distance <= attackRange && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && player.activeInHierarchy)
        {
            target = player.transform;
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage: " + damage);

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