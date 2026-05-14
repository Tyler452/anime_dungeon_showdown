using UnityEngine;

public class CharacterB : MonoBehaviour, IDamageableCharacter
{
    [Header("Special Ability")] public float specialDamage = 100f;
    public float cooldown = 5f;
    public GameObject abilityPrefab;

    private float cooldownTimer;

    void Update()
    {
        if (Input.GetMouseButton(1) && cooldownTimer <= 0f) // Right mouse button for special ability
        {
            UseSpecialAbility();
            cooldownTimer = cooldown; // Reset cooldown
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void UseSpecialAbility()
    {
        if (abilityPrefab != null)
        {
            // Instantiate the ability prefab at the character's location
            Instantiate(abilityPrefab, transform.position, transform.rotation);
            Debug.Log($"Special ability used with {specialDamage} damage!");
        }
    }

    // Implementing the interface method
    public void AddDamageToRandomAbility(float damage)
    {
        // Increase the special ability's damage
        specialDamage += damage;

        Debug.Log($"CharacterB's special damage increased by {damage}. New special damage: {specialDamage}");
    }
}