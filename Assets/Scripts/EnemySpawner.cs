using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")] public GameObject enemyPrefab;

    [Header("Player Targets")] public Transform[] playerTargets; // Must be assigned or detected automatically

    [Header("Wave Settings")] public int startingEnemiesPerWave = 5; // Base number of enemies in the first wave
    public int additionalEnemiesPerWave = 1; // Additional enemies added per wave
    public float timeBetweenWaves = 3f; // Wait time between waves
    public float timeBetweenSpawns = 0.35f; // Time delay between each enemy spawn
    public float minSpawnRadius = 6f; // Minimum distance from player for enemy spawn
    public float maxSpawnRadius = 10f; // Maximum distance from player for enemy spawn

    private int currentWave = 0; // Tracks the current wave
    private int aliveEnemies = 0; // Tracks how many enemies are currently alive

    void Start()
    {
        // Automatically find players if they are not manually set
        AutoFindPlayers();

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: enemyPrefab is not assigned in the Inspector!");
            return;
        }

        if (playerTargets == null || playerTargets.Length == 0)
        {
            Debug.LogError("EnemySpawner: No player targets found or assigned. Spawner will not work!");
            return;
        }

        // Start the spawning loop
        StartCoroutine(WaveLoop());
    }

    void AutoFindPlayers()
    {
        if (playerTargets != null && playerTargets.Length > 0) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            playerTargets = new Transform[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playerTargets[i] = players[i].transform;
            }

            Debug.Log("EnemySpawner: Automatically assigned " + players.Length + " player targets.");
        }
        else
        {
            Debug.LogWarning("EnemySpawner: No players found with the 'Player' tag!");
        }
    }

    IEnumerator WaveLoop()
    {
        while (true) // Loop continuously
        {
            // Wait for all enemies to die before starting the next wave
            while (aliveEnemies > 0)
            {
                yield return null; // Wait for the next frame
            }

            yield return new WaitForSeconds(timeBetweenWaves); // Delay before the next wave

            // Increment the wave
            currentWave++;
            int enemiesThisWave = startingEnemiesPerWave + (currentWave - 1) * additionalEnemiesPerWave;

            Debug.Log("Starting Wave " + currentWave + ": Spawning " + enemiesThisWave + " enemies.");

            for (int i = 0; i < enemiesThisWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns); // Delay between enemy spawns
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Cannot spawn an enemy because enemyPrefab is null!");
            return;
        }

        Transform target = GetRandomValidPlayer();
        if (target == null)
        {
            Debug.LogWarning("EnemySpawner: No valid player target found. Skipping spawn.");
            return;
        }

        // Calculate a random position within the spawn ring
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 spawnPosition = target.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Spawn the enemy at the calculated position
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        aliveEnemies++;

        Debug.Log("Spawned enemy at position: " + spawnPosition);

        // Add a death notification if the enemy has an EnemyAI script
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.target = target; // Assign a target to the enemy
        }
        else
        {
            Debug.LogWarning("EnemySpawner: Spawned enemy does not have an EnemyAI component.");
        }
    }

    Transform GetRandomValidPlayer()
    {
        if (playerTargets == null || playerTargets.Length == 0)
            return null;

        int attempts = 10; // Safeguard to prevent infinite loop if no valid player is found
        while (attempts-- > 0)
        {
            Transform candidate = playerTargets[Random.Range(0, playerTargets.Length)];
            if (candidate != null && candidate.gameObject.activeInHierarchy)
                return candidate;
        }

        return null; // No valid player target found
    }

    // Call this method from EnemyAI or elsewhere when an enemy dies
    public void NotifyEnemyDied()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        Debug.Log("Enemy died. Enemies remaining: " + aliveEnemies);
    }
}