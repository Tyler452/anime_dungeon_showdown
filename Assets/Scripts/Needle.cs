using UnityEngine;
using System.Collections;

public class Needle : MonoBehaviour
{
    [Header("Needle Settings")]
    public float damage = 20f;
    public float explosionRadius = 4f;
    public float explosionDelay = 2f;

    [Header("Effects")]
    public GameObject explosionEffect;

    private bool exploded = false;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponent<EnemyAI>();

        if (enemy != null && !exploded)
        {
            enemy.TakeDamage(damage);
            Explode();
        }
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);

        if (!exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}