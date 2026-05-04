// using UnityEngine;
// using System.Collections.Generic;
//
// public class AbilityHitbox : MonoBehaviour
// {
//     [Header("Damage")]
//     public float damage = 10f;
//     public float lifetime = 0.2f;
//     public bool destroyOnFirstHit = false;
//     public bool singleHitPerEnemy = true;
//
//     [Header("Status Effects")]
//     public bool applyBurn = false;
//     public float burnDamage = 2f;
//     public float burnDuration = 3f;
//
//     public bool applyBleed = false;
//     public float bleedDamage = 2f;
//     public float bleedDuration = 3f;
//
//     public bool applyStun = false;
//     public float stunDuration = 1f;
//
//     public bool applySlow = false;
//     public float slowMultiplier = 0.5f;
//     public float slowDuration = 2f;
//
//     private readonly HashSet<EnemyAI> hitEnemies = new HashSet<EnemyAI>();
//
//     void Start()
//     {
//         Destroy(gameObject, lifetime);
//     }
//
//     void OnTriggerEnter(Collider other)
//     {
//         EnemyAI enemy = other.GetComponent<EnemyAI>();
//         if (enemy == null)
//             return;
//
//         if (singleHitPerEnemy && hitEnemies.Contains(enemy))
//             return;
//
//         hitEnemies.Add(enemy);
//
//         enemy.TakeDamage(damage);
//
//         if (applyBurn)
//             enemy.ApplyBurn(burnDamage, burnDuration);
//
//         if (applyBleed)
//             enemy.ApplyBleed(bleedDamage, bleedDuration);
//
//         if (applyStun)
//             enemy.ApplyStun(stunDuration);
//
//         if (applySlow)
//             enemy.ApplySlow(slowMultiplier, slowDuration);
//
//         if (destroyOnFirstHit)
//             Destroy(gameObject);
//     }
// }