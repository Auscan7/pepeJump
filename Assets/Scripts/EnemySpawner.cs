using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Reference to the enemy prefab
    public float spawnInterval = 3f;     // Time between spawns
    public float enemyHealth = 100f;     // Default health for spawned enemies
    public float enemyDamage = 10f;      // Default damage for spawned enemies
    public Transform[] spawnPoints;      // Array of spawn points

    private void Start()
    {
        // Start the repeated spawning of enemies
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // Choose a random spawn point from the array
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate a new enemy at the chosen spawn point's position and rotation
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Set the enemy's health and damage values
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.maxHealth = enemyHealth;
                enemyScript.damage = enemyDamage;
            }
        }
        else
        {
            Debug.LogWarning("Enemy prefab reference is missing!");
        }
    }
}
