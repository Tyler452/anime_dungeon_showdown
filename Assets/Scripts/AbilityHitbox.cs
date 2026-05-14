using UnityEngine;
using System.Collections.Generic;

public class AbilityHitbox : MonoBehaviour
{
    [Header("Damage")] public float damage = 10f;
    public float lifetime = 0.5f;
    public bool destroyOnFirstHit = false;
    public bool singleHitPerEnemy = true;

    [Header("Status Effects")] public bool applyBurn;
    public float burnDamage, burnDuration;

    public bool applySlow;
    public float slowMultiplier, slowDuration;

    public bool applyStun;
    public float stunDuration;

    private HashSet<Collider> hitTargets = new HashSet<Collider>();
    private System.Action<Collider> hitCallback; // Custom callback on hit

    public void Init(float abilityDamage, System.Action<Collider> hitCallback)
    {
        this.damage = abilityDamage;
        this.hitCallback = hitCallback;
        Destroy(gameObject, lifetime); // Destroy after lifetime ends
    }

    void OnTriggerEnter(Collider other)
    {
        if (hitTargets.Contains(other)) return; // Prevent multi-hit if singleHitPerEnemy is enabled

        hitTargets.Add(other);

        // Check if the collider is valid (e.g., has an EnemyAI script)
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Apply general damage
            enemy.TakeDamage(damage);

            // Apply burn
            if (applyBurn)
                enemy.ApplyBurn(burnDamage, burnDuration);

            // Apply slow
            if (applySlow)
                enemy.ApplySlow(slowMultiplier, slowDuration);

            // Apply stun
            if (applyStun)
                enemy.ApplyStun(stunDuration);
        }

        // Call custom logic (e.g., StunProjectile destroying itself)
        hitCallback?.Invoke(other);

        if (destroyOnFirstHit) Destroy(gameObject);
    }
}