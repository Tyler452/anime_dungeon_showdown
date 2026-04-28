// using UnityEngine;
//
// public class DamageChest : MonoBehaviour
// {
//     [Header("Upgrade")]
//     public float damageIncrease = 5f;
//     public bool destroyAfterOpen = true;
//
//     private bool opened = false;
//
//     private void OnTriggerEnter(Collider other)
//     {
//         if (opened) return;
//
//         CharacterA a = other.GetComponent<CharacterA>();
//         if (a != null)
//         {
//             a.AddDamageToRandomAbility(damageIncrease);
//             OpenChest();
//             return;
//         }
//
//         CharacterB b = other.GetComponent<CharacterB>();
//         if (b != null)
//         {
//             b.AddDamageToRandomAbility(damageIncrease);
//             OpenChest();
//             return;
//         }
//     }
//
//     void OpenChest()
//     {
//         opened = true;
//         Debug.Log("Chest opened");
//
//         if (destroyAfterOpen)
//             Destroy(gameObject);
//     }
// }