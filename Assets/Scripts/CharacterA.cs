using UnityEngine;

public class CharacterA : MonoBehaviour
{
    [Header("Base Attack")] public float baseDamage = 50f;
    public float attackRate = 1f;
    public Transform attackPoint;
    public GameObject hitboxPrefab;

    private float attackTimer;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;
                UseBaseAttack();
            }
        }
        else
        {
            attackTimer = attackRate; // Reset immediately when not attacking
        }
    }

    void UseBaseAttack()
    {
        // Spawn ability hitbox
        if (hitboxPrefab != null)
        {
            GameObject hitboxInstance = Instantiate(hitboxPrefab, attackPoint.position, attackPoint.rotation);
            AbilityHitbox hitbox = hitboxInstance.GetComponent<AbilityHitbox>();

            // Initialize the hitbox with damage and effects (if needed)
            if (hitbox != null)
            {
                hitbox.Init(baseDamage, null); // Pass any custom callbacks if needed
            }
        }
    }
}