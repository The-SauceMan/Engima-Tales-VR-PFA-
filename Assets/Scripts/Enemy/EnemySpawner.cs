using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Enemy enemyData;
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool useRandomSpawn = true;
    [SerializeField] private float spawnRadius = 5f;
    
    [Header("Spawning Control")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private float initialSpawnDelay = 1f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    [SerializeField] private int maxEnemiesAtOnce = 5;
    [SerializeField] private bool spawnInfinite = true;
    [SerializeField] private int totalEnemiesToSpawn = 10;
    
    [Header("Randomization Settings")]
    [SerializeField] private bool randomizeEachSpawn = true;
    [SerializeField] private bool overrideHealthCap = true;
    [SerializeField] private bool overrideSizeCap = true;
    
    private int currentEnemyCount = 0;
    private int enemiesSpawnedTotal = 0;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    void Start()
    {
        // Set up spawn point if not assigned
        if (spawnPoint == null)
        {
            spawnPoint = transform;
            Debug.Log("Using spawner position as spawn point: " + spawnPoint.position);
        }
        
        // Validate prefab
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }
        
        // Subscribe to enemy death events
        EnemyFunction.OnEnemyDied += OnEnemyDeath;
        
        // Start spawning if enabled
        if (startOnAwake)
        {
            StartSpawning();
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        EnemyFunction.OnEnemyDied -= OnEnemyDeath;
        
        // Stop any running coroutines
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize spawn area in Scene view
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(GetSpawnPosition(), 0.5f);
        
        if (useRandomSpawn)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.2f);
            Gizmos.DrawWireSphere(spawnPoint != null ? spawnPoint.position : transform.position, spawnRadius);
        }
    }
    
    public void StartSpawning()
    {
        if (!isSpawning && enemyPrefab != null)
        {
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    IEnumerator SpawnRoutine()
    {
        isSpawning = true;
        
        // Initial delay
        yield return new WaitForSeconds(initialSpawnDelay);
        
        while (isSpawning)
        {
            // Check if we should stop spawning
            if (!spawnInfinite && enemiesSpawnedTotal >= totalEnemiesToSpawn)
            {
                Debug.Log($"Reached spawn limit: {totalEnemiesToSpawn} enemies");
                isSpawning = false;
                yield break;
            }
            
            // Check if we can spawn more enemies
            if (currentEnemyCount < maxEnemiesAtOnce)
            {
                SpawnEnemy();
            }
            
            // Random wait between spawns
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }
    
    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        
        // Get spawn position
        Vector3 spawnPosition = GetSpawnPosition();
        
        // Instantiate enemy
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Set enemy data
        EnemyFunction enemyFunction = enemyInstance.GetComponent<EnemyFunction>();
        if (enemyFunction != null)
        {
            if (enemyData != null)
            {
                enemyFunction.SetEnemyData(enemyData);
                
                // If randomization is disabled in enemy data, we can still override here
                if (!randomizeEachSpawn)
                {
                    // Use base values from enemy data
                    Debug.Log("Spawning enemy with base stats");
                }
            }
            else
            {
                Debug.LogWarning("No enemy data assigned! Using prefab defaults.");
            }
        }
        
        // Track enemy
        activeEnemies.Add(enemyInstance);
        currentEnemyCount++;
        enemiesSpawnedTotal++;
        
        Debug.Log($"Spawned enemy #{enemiesSpawnedTotal} | Active: {currentEnemyCount}");
    }
    
    Vector3 GetSpawnPosition()
    {
        Vector3 basePosition = spawnPoint != null ? spawnPoint.position : transform.position;
        
        if (useRandomSpawn)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return basePosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
        
        return basePosition;
    }
    
    void OnEnemyDeath()
    {
        currentEnemyCount--;
        currentEnemyCount = Mathf.Max(0, currentEnemyCount);
        
        // Clean up destroyed enemies from list
        activeEnemies.RemoveAll(enemy => enemy == null);
        
        Debug.Log("Enemy died. Remaining: " + currentEnemyCount);
    }
    
    // Method to spawn an enemy with specific stats
    public void SpawnEnemyWithStats(int health, float size, Color color)
    {
        if (enemyPrefab == null || currentEnemyCount >= maxEnemiesAtOnce) return;
        
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        EnemyFunction enemyFunction = enemyInstance.GetComponent<EnemyFunction>();
        if (enemyFunction != null)
        {
            // Apply caps
            health = Mathf.Min(health, 300);
            size = Mathf.Min(size, 4f);
            
            enemyFunction.SetRandomStats(health, size, color);
        }
        
        activeEnemies.Add(enemyInstance);
        currentEnemyCount++;
        enemiesSpawnedTotal++;
    }
    
    // Public API
    public void SetSpawnInterval(float min, float max)
    {
        minSpawnInterval = Mathf.Max(0.1f, min);
        maxSpawnInterval = Mathf.Max(minSpawnInterval, max);
    }
    
    public int GetCurrentEnemyCount() => currentEnemyCount;
    public int GetTotalSpawned() => enemiesSpawnedTotal;
    public bool IsSpawning() => isSpawning;
}