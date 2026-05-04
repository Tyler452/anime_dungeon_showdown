using UnityEngine;
using System.Collections;

public class CharacterB : MonoBehaviour
{
    [Header("Base Attack")]
    public float baseDamage = 50f;
    public float attackRate = 1f;
    public Transform shootPoint;
    public GameObject shootEffect;
    private float attackTimer;

    [Header("Kamehameha Beam")]
    public KeyCode ultKey = KeyCode.Q;
    public float beamDuration = 5f;
    public float beamCooldown = 5f;
    public float beamDamage = 20f;
    public float beamRange = 30f;

    [Header("Beam Hitbox Size")]
    public float beamWidth = 2f;
    public float beamHeight = 6f;

    public Transform beamSpawn;
    public GameObject beamEffect;
    private bool canUseUlt = true;

    [Header("Stun Projectile")]
    public KeyCode stunKey = KeyCode.E;
    public GameObject stunProjectilePrefab;
    public Transform stunSpawn;
    public float stunSpeed = 20f;
    public float stunCooldown = 5f;
    private bool canUseStun = true;

    [Header("Needler")]
    public KeyCode needleKey = KeyCode.R;
    public GameObject needlePrefab;
    public float needleSpeed = 25f;
    public float needleCooldown = 5f;
    private bool canUseNeedler = true;
    
    
    [Header("UI")]
    public GameplayUI gameplayUI;

    void Update()
    {
        HandleBaseAttack();

        if (Input.GetKeyDown(ultKey) && canUseUlt)
            StartCoroutine(UseKamehameha());

        if (Input.GetKeyDown(stunKey) && canUseStun)
            StartCoroutine(UseStun());

        if (Input.GetKeyDown(needleKey) && canUseNeedler)
            StartCoroutine(UseNeedler());
    }

    void HandleBaseAttack()
    {
        if (Input.GetMouseButton(0))
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;
                BaseAttack();
            }
        }
        else
        {
            attackTimer = attackRate;
        }
    }

    void BaseAttack()
    {
        if (shootEffect != null)
            Instantiate(shootEffect, shootPoint.position, shootPoint.rotation);

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, Mathf.Infinity))
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();

            if (enemy != null)
                enemy.TakeDamage(baseDamage);
        }
    }

    IEnumerator UseKamehameha()
    {
        canUseUlt = false;
        Debug.Log("Beam activated");
        
        if (beamEffect != null)
            beamEffect.SetActive(true);

        float timer = 0f;

        while (timer < beamDuration)
        {
            timer += Time.deltaTime;
            BeamDamageBox();
            yield return null;
        }

        if (beamEffect != null)
            beamEffect.SetActive(false);
        yield return new WaitForSeconds(beamCooldown);
        canUseUlt = true;
    }

    void BeamDamageBox()
    {
        Vector3 center = beamSpawn.position + beamSpawn.forward * (beamRange / 2f);

        Vector3 halfExtents = new Vector3(
            beamWidth / 2f,
            beamHeight / 2f,
            beamRange / 2f
        );

        Collider[] hits = Physics.OverlapBox(
            center,
            halfExtents,
            beamSpawn.rotation
        );

        foreach (Collider hit in hits)
        {
            EnemyAI enemy = hit.GetComponent<EnemyAI>();

            if (enemy != null)
            {
                enemy.TakeDamage(beamDamage * Time.deltaTime);
            }
        }
    }

    IEnumerator UseStun()
    {
        canUseStun = false;

        if (stunProjectilePrefab != null && stunSpawn != null)
        {
            GameObject stun = Instantiate(
                stunProjectilePrefab,
                stunSpawn.position,
                stunSpawn.rotation
            );

            Rigidbody rb = stun.GetComponent<Rigidbody>();

            if (rb != null)
                rb.linearVelocity = stunSpawn.forward * stunSpeed;
        }
        yield return new WaitForSeconds(stunCooldown);
        canUseStun = true;
    }

    IEnumerator UseNeedler()
    {
        canUseNeedler = false;

        if (needlePrefab != null && shootPoint != null)
        {
            GameObject needle = Instantiate(
                needlePrefab,
                shootPoint.position,
                shootPoint.rotation
            );

            Rigidbody rb = needle.GetComponent<Rigidbody>();

            if (rb != null)
                rb.linearVelocity = shootPoint.forward * needleSpeed;
        }
        yield return new WaitForSeconds(needleCooldown);
        canUseNeedler = true;
    }

    void OnEnable()
    {
        canUseUlt = true;
        canUseStun = true;
        canUseNeedler = true;

        if (beamEffect != null)
            beamEffect.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (beamSpawn == null) return;

        Gizmos.color = Color.cyan;

        Vector3 center = beamSpawn.position + beamSpawn.forward * (beamRange / 2f);
        Vector3 size = new Vector3(beamWidth, beamHeight, beamRange);

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, beamSpawn.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = oldMatrix;
    }
}