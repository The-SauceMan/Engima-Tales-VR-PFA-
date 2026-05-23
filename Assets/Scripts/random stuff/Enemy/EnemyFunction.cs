using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class EnemyFunction : MonoBehaviour
{
    public static event Action OnEnemyDied;
    public static event Action<int> OnEnemyDiedWithHealth;
    
    [Header("Enemy Data")]
    [SerializeField] private Enemy enemyData;
    
    [Header("Current Stats")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int initialHealth; // NEW: Store initial health for scoring
    [SerializeField] private float currentSize;
    [SerializeField] private Color currentColor;
    
    [Header("Floating Display")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0);
    [SerializeField] private float textFollowSpeed = 10f;
    [SerializeField] private bool showHealth = true;
    [SerializeField] private bool showPoints = true;
    [SerializeField] private bool showInitialHealth = false; // NEW: Option to show initial health
    
    [Header("Scoring")]
    [SerializeField] private bool enableScoring = true;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private bool useInitialHealthForScoring = true; // NEW: Toggle between initial/current
    
    // Scoring thresholds
    private int healthThreshold1 = 50;   // 1 point: 50-99
    private int healthThreshold2 = 100;  // 2 points: 100-149
    private int healthThreshold3 = 150;  // 3 points: 150-199
    private int healthThreshold4 = 200;  // 4 points: 200+
    
    private Renderer enemyRenderer;
    private Material enemyMaterial;
    private bool isDead = false;
    private GameObject floatingTextObject;
    private TextMeshPro floatingText;
    private Vector3 textWorldPosition;
    
    void Awake()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            enemyMaterial = new Material(enemyRenderer.material);
            enemyRenderer.material = enemyMaterial;
        }
        
        CreateFloatingText();
    }
    
    void CreateFloatingText()
    {
        if (floatingTextPrefab != null)
        {
            floatingTextObject = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
            floatingTextObject.name = gameObject.name + "_FloatingText";
            floatingText = floatingTextObject.GetComponent<TextMeshPro>();
            
            if (floatingText == null)
            {
                Debug.LogWarning("Floating text prefab doesn't have TextMeshPro component!");
                Destroy(floatingTextObject);
                floatingTextObject = null;
            }
            else
            {
                floatingTextObject.transform.SetParent(null);
                floatingText.alignment = TextAlignmentOptions.Center;
                floatingText.fontSize = 3;
                floatingText.enableWordWrapping = false;
                
                UpdateTextPosition();
                UpdateFloatingText();
            }
        }
        else
        {
            Debug.LogWarning("No floating text prefab assigned to enemy!");
        }
    }
    
    void UpdateTextPosition()
    {
        if (floatingTextObject == null) return;
        
        float heightOffset = currentSize * 1.5f + 0.5f;
        textWorldPosition = transform.position + new Vector3(0, heightOffset, 0);
        
        floatingTextObject.transform.position = Vector3.Lerp(
            floatingTextObject.transform.position, 
            textWorldPosition, 
            Time.deltaTime * textFollowSpeed
        );
        
        if (Camera.main != null)
        {
            floatingTextObject.transform.LookAt(
                floatingTextObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }
    
    void UpdateFloatingText()
    {
        if (floatingText == null) return;
        
        string displayText = "";
        
        if (showHealth)
        {
            // Calculate health percentage based on CURRENT health
            float healthPercent = (float)currentHealth / GetMaxHealth();
            
            // Choose color based on current health percentage
            string healthColorHex;
            if (healthPercent > 0.66f)
                healthColorHex = "#00FF00"; // Green
            else if (healthPercent > 0.33f)
                healthColorHex = "#FFFF00"; // Yellow
            else
                healthColorHex = "#FF0000"; // Red
            
            string healthText = $"HP: {currentHealth}";
            
            // Optionally show initial health too
            if (showInitialHealth && initialHealth != currentHealth)
            {
                healthText += $" ({initialHealth})";
            }
            
            displayText += $"<color={healthColorHex}>{healthText}</color>";
        }
        
        if (showHealth && showPoints)
        {
            displayText += "\n";
        }
        
        if (showPoints)
        {
            // Points color based on score value (calculated from INITIAL health)
            string pointsColorHex = GetPointsColorHex(scoreValue);
            displayText += $"<color={pointsColorHex}>Points: {scoreValue}</color>";
        }
        
        floatingText.text = displayText;
        
        float textScale = Mathf.Clamp(currentSize * 0.5f, 0.5f, 2f);
        floatingTextObject.transform.localScale = Vector3.one * textScale;
    }
    
    string GetPointsColorHex(int points)
    {
        switch (points)
        {
            case 4: return "#FF0000";    // Red for 4 points
            case 3: return "#FF00FF";    // Magenta for 3 points
            case 2: return "#00FF00";    // Green for 2 points
            case 1: return "#00FFFF";    // Cyan for 1 point
            default: return "#FFFFFF";   // White for other
        }
    }
    
    void Start()
    {
        if (enemyData != null)
        {
            ApplyRandomEnemyData();
        }
        else
        {
            Debug.LogWarning("Enemy data not assigned! Using default values.");
            ApplyDefaultStats();
        }
        
        // Calculate score value based on initial health
        CalculateScoreValueFromInitialHealth();
        
        if (floatingText != null)
        {
            UpdateFloatingText();
        }
        
        Debug.Log($"Enemy spawned - Initial Health: {initialHealth}, Current Health: {currentHealth}, Points: {scoreValue}");
    }
    
    void Update()
    {
        if (floatingTextObject != null && !isDead)
        {
            UpdateTextPosition();
        }
    }
    
    // NEW: Calculate score from INITIAL health (never changes)
    void CalculateScoreValueFromInitialHealth()
    {
        int healthForScoring = useInitialHealthForScoring ? initialHealth : currentHealth;
        
        if (healthForScoring >= healthThreshold4)        // 200+
            scoreValue = 4;
        else if (healthForScoring >= healthThreshold3)   // 150-199
            scoreValue = 3;
        else if (healthForScoring >= healthThreshold2)   // 100-149
            scoreValue = 2;
        else if (healthForScoring >= healthThreshold1)   // 50-99
            scoreValue = 1;
        else                                             // Below 50
            scoreValue = 1; // Base score
    }
    
    // OLD: Remove this or keep for reference
    void CalculateScoreValueFromCurrentHealth()
    {
        if (currentHealth >= healthThreshold4)
            scoreValue = 4;
        else if (currentHealth >= healthThreshold3)
            scoreValue = 3;
        else if (currentHealth >= healthThreshold2)
            scoreValue = 2;
        else if (currentHealth >= healthThreshold1)
            scoreValue = 1;
        else
            scoreValue = 1;
    }
    
    void ApplyDefaultStats()
    {
        initialHealth = 100; // NEW: Set initial health
        currentHealth = initialHealth;
        currentSize = 1f;
        currentColor = Color.red;
        ApplyVisuals();
    }
    
    public void SetEnemyData(Enemy newEnemy)
    {
        enemyData = newEnemy;
        if (enemyData != null)
        {
            ApplyRandomEnemyData();
        }
    }
    
    void ApplyRandomEnemyData()
    {
        if (enemyData == null) return;
        
        EnemyStats stats = enemyData.GetRandomStats();
        
        // Set both initial and current health
        initialHealth = Mathf.Min(stats.health, enemyData.healthCap);
        currentHealth = initialHealth;
        
        currentSize = Mathf.Min(stats.size, enemyData.sizeCap);
        currentColor = stats.color;
        
        ApplyVisuals();
        
        // Calculate score from initial health (never changes)
        CalculateScoreValueFromInitialHealth();
        
        if (floatingText != null)
        {
            UpdateFloatingText();
        }
        
        Debug.Log($"Enemy spawned - Initial Health: {initialHealth}, Size: {currentSize}, Points: {scoreValue}");
    }
    
    void ApplyVisuals()
    {
        transform.localScale = Vector3.one * currentSize;
        
        if (enemyMaterial != null)
        {
            enemyMaterial.color = currentColor;
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead || currentHealth <= 0) return;
        
        currentHealth -= damage;
        
        // IMPORTANT: DO NOT recalculate score value here!
        // Score is based on initial health, not current health
        
        if (floatingText != null)
        {
            // Only update the HP display, not the points
            UpdateFloatingText();
            ShowDamagePopup(damage);
        }
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{initialHealth}, Points: {scoreValue} (fixed)");
        
        if (enemyMaterial != null)
        {
            StartCoroutine(DamageFlash());
        }
        
        if (currentHealth <= 0)
            Die();
    }
    
    void ShowDamagePopup(int damage)
    {
        if (floatingTextPrefab != null)
        {
            GameObject damagePopup = Instantiate(
                floatingTextPrefab, 
                transform.position + textOffset + new Vector3(0, 1f, 0), 
                Quaternion.identity
            );
            
            TextMeshPro damageText = damagePopup.GetComponent<TextMeshPro>();
            if (damageText != null)
            {
                damageText.text = $"<color=#FF0000>-{damage}</color>";
                damageText.alignment = TextAlignmentOptions.Center;
                damageText.fontSize = 2;
                
                if (Camera.main != null)
                {
                    damagePopup.transform.LookAt(
                        damagePopup.transform.position + Camera.main.transform.rotation * Vector3.forward,
                        Camera.main.transform.rotation * Vector3.up
                    );
                }
                
                Destroy(damagePopup, 0.5f);
                StartCoroutine(AnimateDamagePopup(damagePopup));
            }
        }
    }
    
    IEnumerator AnimateDamagePopup(GameObject popup)
    {
        float duration = 0.5f;
        float timer = 0f;
        Vector3 startPos = popup.transform.position;
        
        while (timer < duration && popup != null)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            popup.transform.position = startPos + new Vector3(0, progress * 2f, 0);
            
            TextMeshPro text = popup.GetComponent<TextMeshPro>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - progress;
                text.color = color;
            }
            
            yield return null;
        }
    }
    
    IEnumerator DamageFlash()
    {
        if (enemyMaterial == null) yield break;
        
        Color originalColor = enemyMaterial.color;
        enemyMaterial.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        enemyMaterial.color = originalColor;
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Score value is already calculated from initial health and never changed
        Debug.Log($"{gameObject.name} died! Initial Health: {initialHealth}, Current Health: {currentHealth}, Points: {scoreValue}");
        
        // Update floating text with death message
        if (floatingText != null)
        {
            string deathColor = GetPointsColorHex(scoreValue);
            floatingText.text = $"<color=#FF0000>DEAD!</color>\n<color={deathColor}>+{scoreValue} points</color>";
        }
        
        // Send INITIAL health to score manager (for scoring)
        // Or send current health for tracking, but scoring uses initial
        if (VRWatchScoreManager.Instance != null)
        {
            // Pass initial health for proper scoring
            VRWatchScoreManager.Instance.ScoreEnemyKill(initialHealth);
        }
        else
        {
            Debug.LogWarning("VRWatchScoreManager.Instance is null! Score won't be recorded.");
        }
        
        // Trigger events - pass initial health for consistency
        OnEnemyDiedWithHealth?.Invoke(initialHealth);
        OnEnemyDied?.Invoke();
        
        // Start death effects
        StartCoroutine(DeathEffect());
    }
    
    IEnumerator DeathEffect()
    {
        float deathTimer = 0.5f;
        Vector3 originalScale = transform.localScale;
        float textAlpha = 1f;
        
        while (deathTimer > 0)
        {
            deathTimer -= Time.deltaTime;
            
            transform.localScale = originalScale * (deathTimer / 0.5f);
            
            if (floatingText != null)
            {
                textAlpha = deathTimer / 0.5f;
                Color textColor = floatingText.color;
                textColor.a = textAlpha;
                floatingText.color = textColor;
            }
            
            yield return null;
        }
        
        if (floatingTextObject != null)
        {
            Destroy(floatingTextObject);
        }
        
        Destroy(gameObject);
    }
    
    // Public method to calculate score from INITIAL health
    public int CalculateScoreFromInitialHealth()
    {
        if (initialHealth >= healthThreshold4)
            return 4;
        else if (initialHealth >= healthThreshold3)
            return 3;
        else if (initialHealth >= healthThreshold2)
            return 2;
        else if (initialHealth >= healthThreshold1)
            return 1;
        else
            return 1;
    }
    
    // Get enemy tier based on INITIAL health
    public string GetEnemyTier()
    {
        if (initialHealth >= healthThreshold4)
            return "BOSS";
        else if (initialHealth >= healthThreshold3)
            return "Elite";
        else if (initialHealth >= healthThreshold2)
            return "Strong";
        else if (initialHealth >= healthThreshold1)
            return "Normal";
        else
            return "Weak";
    }
    
    // Public getters
    public int GetCurrentHealth() => currentHealth;
    public int GetInitialHealth() => initialHealth; // NEW: Get initial health
    public int GetMaxHealth() => enemyData != null ? Mathf.Min(enemyData.maxHealth, enemyData.healthCap) : 100;
    public float GetCurrentSize() => currentSize;
    public Color GetCurrentColor() => currentColor;
    public int GetScoreValue() => scoreValue;
    
    public void SetRandomStats(int health, float size, Color color)
    {
        // Set both initial and current health
        initialHealth = Mathf.Min(health, 300);
        currentHealth = initialHealth;
        
        currentSize = Mathf.Min(size, 4f);
        currentColor = color;
        
        // Calculate score from initial health
        CalculateScoreValueFromInitialHealth();
        
        ApplyVisuals();
        
        if (floatingText != null)
        {
            UpdateFloatingText();
        }
    }
    
    // Method to heal enemy (if needed) - doesn't affect score
    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, GetMaxHealth());
        
        if (floatingText != null)
        {
            UpdateFloatingText();
        }
        
        Debug.Log($"{gameObject.name} healed by {amount}. Health: {currentHealth}/{initialHealth}");
    }
    
    void OnDisable()
    {
        if (floatingTextObject != null)
        {
            Destroy(floatingTextObject);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        Vector3 labelPos = transform.position + Vector3.up * (currentSize + 2f);
        
        #if UNITY_EDITOR
        string info = $"{GetEnemyTier()} Enemy\n" +
                     $"HP: {currentHealth}/{initialHealth}\n" +
                     $"Points: {scoreValue} (fixed)";
        
        UnityEditor.Handles.Label(labelPos, info);
        #endif
    }
}