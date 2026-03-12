using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float spawnRate = 5f;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // limit total enemies in the scene
        if (FindObjectsOfType<EnemyAI>().Length >= maxEnemies)
            return;

        // pick random point around spawner
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPosition = transform.position + randomOffset;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}