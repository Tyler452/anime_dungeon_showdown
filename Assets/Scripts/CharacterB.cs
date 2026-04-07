using UnityEngine;
using System.Collections;

public class CharacterB : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Normal Attack")]
    public GameObject basicVFX;
    public AudioClip basicSFX;

    [Header("Beam")]
    public KeyCode ultKey = KeyCode.Q;
    public GameObject beamVFX;
    public AudioClip beamSFX;
    public float beamDuration = 5f;
    public float beamCooldown = 25f;
    public float beamDamage = 20f;
    public float beamRange = 30f;
    private bool canUseUlt = true;

    [Header("Stun")]
    public KeyCode stunKey = KeyCode.E;
    public GameObject stunVFX;
    public AudioClip stunSFX;
    public float stunRange = 20f;
    public float stunDamage = 10f;
    public float slowAmount = 0.5f;
    public float slowDuration = 2f;
    private bool canUseStun = true;

    [Header("Needler")]
    public KeyCode needleKey = KeyCode.R;
    public GameObject needleVFX;
    public AudioClip needleSFX;
    public GameObject explosionVFX;
    public AudioClip explosionSFX;
    public GameObject needlePrefab;
    public float needleSpeed = 25f;
    public float explosionDelay = 2f;
    public float explosionRadius = 4f;
    public float needleDamage = 25f;
    private bool canUseNeedler = true;

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
            StartCoroutine(UseBeam());

        if (Input.GetKeyDown(stunKey) && canUseStun)
            StartCoroutine(UseStun());

        if (Input.GetKeyDown(needleKey) && canUseNeedler)
            StartCoroutine(UseNeedler());
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

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 4f))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(8f);
        }
    }

    IEnumerator UseBeam()
    {
        canUseUlt = false;

        PlaySFX(beamSFX);

        float timer = 0f;

        while (timer < beamDuration)
        {
            timer += Time.deltaTime;

            PlayVFX(beamVFX, transform.position + transform.forward * 3f);

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, beamRange))
            {
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                    enemy.TakeDamage(beamDamage * Time.deltaTime);
            }

            yield return null;
        }

        yield return new WaitForSeconds(beamCooldown);
        canUseUlt = true;
    }

    IEnumerator UseStun()
    {
        canUseStun = false;

        PlayVFX(stunVFX, transform.position + transform.forward * 2f);
        PlaySFX(stunSFX);

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, stunRange))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(stunDamage);
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }

        yield return new WaitForSeconds(10f);
        canUseStun = true;
    }

    IEnumerator UseNeedler()
    {
        canUseNeedler = false;

        GameObject needle = Instantiate(
            needlePrefab,
            transform.position + transform.forward,
            transform.rotation
        );

        PlayVFX(needleVFX, needle.transform.position);
        PlaySFX(needleSFX);

        Rigidbody rb = needle.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = transform.forward * needleSpeed;

        yield return new WaitForSeconds(explosionDelay);

        Vector3 pos = needle.transform.position;

        PlayVFX(explosionVFX, pos);
        PlaySFX(explosionSFX);

        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius);

        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(needleDamage);
        }

        Destroy(needle);

        yield return new WaitForSeconds(6f);
        canUseNeedler = true;
    }

    public void Dodge()
    {
        PlaySFX(dodgeSFX);
    }
}