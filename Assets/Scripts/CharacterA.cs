using UnityEngine;
using System.Collections;

public class CharacterA : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Normal Attack")]
    public GameObject basicVFX;
    public AudioClip basicSFX;

    [Header("Ultimate")]
    public KeyCode ultKey = KeyCode.Q;
    public GameObject ultVFX;
    public AudioClip ultSFX;
    public float ultRadius = 5f;
    public float ultDamage = 9999f;
    public float ultCooldown = 20f;
    private bool canUseUlt = true;

    [Header("Flamethrower")]
    public KeyCode flameKey = KeyCode.E;
    public GameObject flameVFX;
    public AudioClip flameSFX;
    public float flameRange = 6f;
    public float flameDamage = 10f;
    public float burnDamage = 2f;
    public float burnDuration = 3f;
    public float flameCooldown = 8f;
    private bool canUseFlame = true;

    [Header("Grapple")]
    public KeyCode grappleKey = KeyCode.R;
    public GameObject grappleVFX;
    public AudioClip grappleSFX;
    public float grappleRange = 15f;
    public float grappleSpeed = 25f;
    public float grappleStopDistance = 3f;
    public float grappleCooldown = 10f;
    private bool canUseGrapple = true;

    [Header("Movement")]
    public AudioClip dodgeSFX;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            BasicAttack();

        if (Input.GetKeyDown(ultKey) && canUseUlt)
            StartCoroutine(UseUlt());

        if (Input.GetKeyDown(flameKey) && canUseFlame)
            StartCoroutine(UseFlame());

        if (Input.GetKeyDown(grappleKey) && canUseGrapple)
            StartCoroutine(UseGrapple());
    }

    void PlayVFX(GameObject vfx, Vector3 pos)
    {
        if (vfx != null)
        {
            GameObject obj = Instantiate(vfx, pos, Quaternion.identity);
            Destroy(obj, 2f);
        }
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    void BasicAttack()
    {
        PlayVFX(basicVFX, transform.position + transform.forward * 1.5f);
        PlaySFX(basicSFX);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 3f))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(10f);
        }
    }

    IEnumerator UseUlt()
    {
        canUseUlt = false;

        PlayVFX(ultVFX, transform.position);
        PlaySFX(ultSFX);

        Collider[] hits = Physics.OverlapSphere(transform.position, ultRadius);

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(ultDamage);
        }

        yield return new WaitForSeconds(ultCooldown);
        canUseUlt = true;
    }

    IEnumerator UseFlame()
    {
        canUseFlame = false;

        PlayVFX(flameVFX, transform.position + transform.forward * 2f);
        PlaySFX(flameSFX);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1f, transform.forward, flameRange);

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

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, grappleRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                PlayVFX(grappleVFX, hit.point);
                PlaySFX(grappleSFX);

                Transform target = enemy.transform;

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

    public void Dodge()
    {
        PlaySFX(dodgeSFX);
    }
}