using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;   // Array of enemy prefabs to spawn from this spawner
    public float spawnDelay = 10f;      // Time delay before a new enemy is spawned after the previous one is killed
    public Transform spawnPoint;        // Spawn point for this spawner

    private GameObject currentEnemy;    // The currently spawned enemy

    private void Start()
    {
        // Spawn an enemy at the start
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (currentEnemy == null && enemyPrefabs.Length > 0)
        {
            // Choose a random enemy prefab to spawn
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Instantiate the enemy at the spawn point
            currentEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Start listening for the enemy's death
            Enemy enemyScript = currentEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath()
    {
        // Stop listening to the enemy's death event
        Enemy enemyScript = currentEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath -= HandleEnemyDeath;
        }

        // Destroy the current enemy
        Destroy(currentEnemy);

        // Start the spawn delay coroutine
        StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(spawnDelay);

        // Spawn a new enemy after the delay
        SpawnEnemy();
    }
}


