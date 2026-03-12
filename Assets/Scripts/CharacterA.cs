using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class CharacterA : MonoBehaviour
{
    [Header("Ultimate (AOE Nuke)")]
    public KeyCode ultKey = KeyCode.Q;
    public float ultRadius = 5f;
    public float ultDamage = 9999f; // basically one-shot
    public float ultCooldown = 20f;
    private bool canUseUlt = true;

    [Header("Flamethrower")]
    public KeyCode flameKey = KeyCode.E;
    public float flameRange = 6f;
    public float flameDamage = 10f;
    public float burnDamage = 2f;
    public float burnDuration = 3f;
    public float flameCooldown = 8f;
    private bool canUseFlame = true;

    [Header("Grapple (Scorpion Spear)")]
    public KeyCode grappleKey = KeyCode.R;
    public float grappleRange = 15f;
    public float grappleSpeed = 25f;
    public float grappleStopDistance = 3f;
    public float grappleCooldown = 10f;
    private bool canUseGrapple = true;

    void Update()
    {
        if (Input.GetKeyDown(ultKey) && canUseUlt)
            StartCoroutine(UseUlt());

        if (Input.GetKeyDown(flameKey) && canUseFlame)
            StartCoroutine(UseFlamethrower());

        if (Input.GetKeyDown(grappleKey) && canUseGrapple)
            StartCoroutine(UseGrapple());
    }

    IEnumerator UseUlt()
    {
        canUseUlt = false;
        Debug.Log("AOE Nuke used");

        // check everything inside the radius
        Collider[] hits = Physics.OverlapSphere(transform.position, ultRadius);

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(ultDamage);
            }
        }

        yield return new WaitForSeconds(ultCooldown);

        canUseUlt = true;
    }

    IEnumerator UseFlamethrower()
    {
        canUseFlame = false;
        Debug.Log("Flamethrower started");

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            1f,
            transform.forward,
            flameRange
        );

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(flameDamage);
                enemy.ApplyBurn(burnDamage, burnDuration);
            }
        }

        yield return new WaitForSeconds(flameCooldown);

        canUseFlame = true;
    }

    IEnumerator UseGrapple()
    {
        canUseGrapple = false;
        Debug.Log("Grapple launched");

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, grappleRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                Transform target = enemy.transform;

                // pull enemy toward player but stop a few units away
                while (Vector3.Distance(target.position, transform.position) > grappleStopDistance)
                {
                    target.position = Vector3.MoveTowards(
                        target.position,
                        transform.position,
                        grappleSpeed * Time.deltaTime
                    );

                    yield return null;
                }
            }
        }

        yield return new WaitForSeconds(grappleCooldown);

        canUseGrapple = true;
    }

    void OnDrawGizmosSelected()
    {
        // draw ult radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ultRadius);
    }
}