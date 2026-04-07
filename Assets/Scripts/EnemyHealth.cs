using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    // called when abilities hit the enemy
    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log("Enemy took damage: " + damage);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}