using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy")]
public class Enemy : ScriptableObject
{
    [Header("Health Settings")]
    public int minHealth = 50;
    public int maxHealth = 150;
    [Range(1, 300)] public int healthCap = 300;
    
    [Header("Size Settings")]
    public float minSize = 0.5f;
    public float maxSize = 2f;
    [Range(0.1f, 4f)] public float sizeCap = 4f;
    
    [Header("Color Settings")]
    public Color baseColor = Color.red;
    public Color randomColorMin = Color.red;
    public Color randomColorMax = new Color(1f, 0.5f, 0f); // Orange
    public bool useRandomColors = true;
    
    // Helper method to get random enemy stats
    public EnemyStats GetRandomStats()
    {
        EnemyStats stats = new EnemyStats();
        
        // Random health (capped)
        stats.health = Mathf.Min(Random.Range(minHealth, maxHealth + 1), healthCap);
        
        // Random size (capped)
        stats.size = Mathf.Min(Random.Range(minSize, maxSize), sizeCap);
        
        // Random color
        if (useRandomColors)
        {
            stats.color = new Color(
                Random.Range(randomColorMin.r, randomColorMax.r),
                Random.Range(randomColorMin.g, randomColorMax.g),
                Random.Range(randomColorMin.b, randomColorMax.b),
                1f
            );
        }
        else
        {
            stats.color = baseColor;
        }
        
        return stats;
    }
}

// Data structure to hold randomized enemy stats
[System.Serializable]
public struct EnemyStats
{
    public int health;
    public float size;
    public Color color;
}