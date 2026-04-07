using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 5f;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;

    private float timer;
    private int currentEnemies = 0;

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
        if (currentEnemies >= maxEnemies) return;

        Vector3 offset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Instantiate(enemyPrefab, transform.position + offset, Quaternion.identity);
        currentEnemies++;
    }

    public void NotifyEnemyDied()
    {
        currentEnemies--;
    }
}