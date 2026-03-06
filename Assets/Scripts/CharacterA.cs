using UnityEngine;
using System.Collections;

public class CharacterA : MonoBehaviour
{
    [Header("Ultimate (AOE Nuke)")]
    public KeyCode ultKey = KeyCode.Q;
    public float ultRadius = 5f;
    public float ultDamage = 9999f;       // basically one-shot
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
        Debug.Log("AOE Nuke used!");

        // hits everything in a small radius
        Collider[] hits = Physics.OverlapSphere(transform.position, ultRadius);
        foreach (var hit in hits)
        {
            // here I’d check if it's an enemy and then deal damage
            Debug.Log("Hit " + hit.name + " with nuke!");
        }

        yield return new WaitForSeconds(ultCooldown);
        canUseUlt = true;
    }

    IEnumerator UseFlamethrower()
    {
        canUseFlame = false;
        Debug.Log("Flamethrower started");

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1f, transform.forward, flameRange);
        foreach (var hit in hits)
        {
            Debug.Log("Burning " + hit.collider.name);
            // could apply DOT here
        }

        yield return new WaitForSeconds(flameCooldown);
        canUseFlame = true;
    }

    IEnumerator UseGrapple()
    {
        canUseGrapple = false;
        Debug.Log("Grapple launched!");

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, grappleRange))
        {
            Debug.Log("Hit " + hit.collider.name + " with grapple");
            // simple pull toward player (not all the way)
            Vector3 pullPos = transform.position + transform.forward * 3f;
            if (hit.rigidbody != null)
            {
                hit.rigidbody.MovePosition(Vector3.MoveTowards(hit.transform.position, pullPos, grappleSpeed * Time.deltaTime));
            }
        }

        yield return new WaitForSeconds(grappleCooldown);
        canUseGrapple = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ultRadius);
    }
}