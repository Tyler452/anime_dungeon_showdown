using UnityEngine;

[RequireComponent(typeof(AbilityHitbox))]
public class StunProjectile : MonoBehaviour
{
    [Header("Stun Settings")] public float damage = 10f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    public float lifeTime = 3f;

    [Header("Effects")] public GameObject hitEffect;

    private AbilityHitbox hitbox;

    void Start()
    {
        hitbox = GetComponent<AbilityHitbox>();
        hitbox.Init(damage, hitCallback: OnHit);
        Destroy(gameObject, lifeTime); // Auto-lifetime timer
    }

    private void OnHit(Collider target)
    {
        EnemyAI enemy = target.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Apply stun and slow effects
            enemy.ApplySlow(slowAmount, slowDuration);

            // Optional: Explosion, sound, or visuals
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}