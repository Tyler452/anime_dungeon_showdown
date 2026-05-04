using UnityEngine;

public class CharacterA : MonoBehaviour
{
    [Header("Base Attack (Hold Mouse)")]
    public float baseDamage = 50f;
    public float attackRate = 1f;
    public float attackRadius = 2f;
    public Transform attackPoint;
    public GameObject slashEffect;

    private float attackTimer;
    
    
    //UI
    
    public GameplayUI gameplayUI;
    void Update()
    {
        HandleBaseAttack();
    }

    void HandleBaseAttack()
    {
        if (Input.GetMouseButton(0))
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;
                BaseAttack();
            }
        }
        else
        {
            attackTimer = attackRate;
        }
    }

    void BaseAttack()
    {
        if (slashEffect != null)
            Instantiate(slashEffect, attackPoint.position, attackPoint.rotation);

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius);

        foreach (var hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;

            if (Vector3.Dot(transform.forward, dir) > 0.5f)
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();

                if (enemy != null)
                    enemy.TakeDamage(baseDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}