using UnityEngine;

public class DamageChest : MonoBehaviour
{
    [Header("Upgrade Settings")] [Tooltip("Amount of damage increase granted by the chest.")]
    public float damageIncrease = 5f;

    [Tooltip("Should the chest be destroyed after opening?")]
    public bool destroyAfterOpen = true;

    private bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (opened) return;

        // Attempt to find a component with the method to add damage
        IDamageableCharacter character = other.GetComponent<IDamageableCharacter>();
        if (character != null)
        {
            character.AddDamageToRandomAbility(damageIncrease);
            OpenChest();
        }
    }

    private void OpenChest()
    {
        opened = true;
        Debug.Log("Chest opened");

        if (destroyAfterOpen)
            Destroy(gameObject);
    }
}

// Interface for characters that can have damage abilities
public interface IDamageableCharacter
{
    void AddDamageToRandomAbility(float damage);
}

