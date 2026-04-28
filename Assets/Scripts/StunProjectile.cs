using UnityEngine;

public class StunProjectile : MonoBehaviour
{
    [Header("Stun Settings")]
    public float damage = 10f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    public float lifeTime = 3f;

    [Header("Effects")]
    public GameObject hitEffect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ApplySlow(slowAmount, slowDuration);

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}