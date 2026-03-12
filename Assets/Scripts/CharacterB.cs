using UnityEngine;
using System.Collections;

public class CharacterB : MonoBehaviour
{
    [Header("Kamehameha (Ult)")]
    public KeyCode ultKey = KeyCode.Q;
    public float beamDuration = 5f;
    public float beamCooldown = 25f;
    public float beamDamage = 20f;
    public float beamRange = 30f;
    private bool canUseUlt = true;

    [Header("Stun Blast")]
    public KeyCode stunKey = KeyCode.E;
    public float stunCooldown = 10f;
    public float stunRange = 20f;
    public float stunDamage = 10f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    private bool canUseStun = true;

    [Header("Needler")]
    public KeyCode needleKey = KeyCode.R;
    public float needleCooldown = 6f;
    public float needleSpeed = 25f;
    public float needleExplosionRadius = 4f;
    public float needleDamage = 25f;
    public float explosionDelay = 2f;
    public GameObject needlePrefab;
    private bool canUseNeedler = true;

    void Update()
    {
        if (Input.GetKeyDown(ultKey) && canUseUlt)
            StartCoroutine(UseKamehameha());

        if (Input.GetKeyDown(stunKey) && canUseStun)
            StartCoroutine(UseStunBlast());

        if (Input.GetKeyDown(needleKey) && canUseNeedler)
            StartCoroutine(UseNeedler());
    }

    IEnumerator UseKamehameha()
    {
        canUseUlt = false;
        Debug.Log("Kamehameha activated");

        float timer = 0f;

        while (timer < beamDuration)
        {
            timer += Time.deltaTime;

            // continuous ray while beam is active
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, beamRange))
            {
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

                if (enemy != null)
                {
                    enemy.TakeDamage(beamDamage * Time.deltaTime);
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(beamCooldown);

        canUseUlt = true;
    }

    IEnumerator UseStunBlast()
    {
        canUseStun = false;
        Debug.Log("Stun blast fired");

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, stunRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(stunDamage);
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }

        yield return new WaitForSeconds(stunCooldown);

        canUseStun = true;
    }

    IEnumerator UseNeedler()
    {
        canUseNeedler = false;
        Debug.Log("Needler fired");

        if (needlePrefab != null)
        {
            GameObject needle = Instantiate(
                needlePrefab,
                transform.position + transform.forward * 1f,
                transform.rotation
            );

            Rigidbody rb = needle.GetComponent<Rigidbody>();

            if (rb != null)
                rb.linearVelocity = transform.forward * needleSpeed;

            StartCoroutine(NeedleExplosion(needle));
        }

        yield return new WaitForSeconds(needleCooldown);

        canUseNeedler = true;
    }

    IEnumerator NeedleExplosion(GameObject needle)
    {
        yield return new WaitForSeconds(explosionDelay);

        if (needle == null) yield break;

        Vector3 explosionPos = needle.transform.position;

        Collider[] hits = Physics.OverlapSphere(
            explosionPos,
            needleExplosionRadius
        );

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(needleDamage);
            }
        }

        Destroy(needle);
    }

    void OnDrawGizmosSelected()
    {
        // draw ranges in editor for debugging
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * beamRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * stunRange);
    }
}