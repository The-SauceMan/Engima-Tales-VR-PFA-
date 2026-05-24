using UnityEngine;
using TMPro;
using System.Collections;

public class VRWatchScoreManager : MonoBehaviour
{
    [Header("Watch Display - 3D Text Objects")]
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private TextMeshPro killsText;
    [SerializeField] private TextMeshPro highScoreText;
    [SerializeField] private TextMeshPro pointsPerKillText; // Optional: Show points from last kill
    
    [Header("Score Settings")]
    [SerializeField] private int baseScorePerKill = 1;
    [SerializeField] private int healthThreshold1 = 50;   // 1 point: 50-99
    [SerializeField] private int healthThreshold2 = 100;  // 2 points: 100-149
    [SerializeField] private int healthThreshold3 = 150;  // 3 points: 150-199
    [SerializeField] private int healthThreshold4 = 200;  // 4 points: 200+
    [SerializeField] private Color normalScoreColor = Color.white;
    [SerializeField] private Color bonusScoreColor = Color.yellow;
    [SerializeField] private Color maxBonusColor = Color.red; // For 4 points
    [SerializeField] private float bonusDisplayTime = 1f;
    
    // SINGLETON
    public static VRWatchScoreManager Instance { get; private set; }
    
    // Scoring
    private int currentScore = 0;
    private int totalKills = 0;
    private int highScore = 0;
    
    // Tracking
    private int lastKillPoints = 0;
    private int lastKillHealth = 0;
    private float lastKillTime = 0f;
    
    // Display
    private Coroutine bonusDisplayCoroutine;
    private Color originalScoreColor;
    private bool isSubscribed = false;
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (scoreText != null)
            originalScoreColor = scoreText.color;
    }
    
    void Start()
    {
        LoadHighScore();
        UpdateWatchDisplay();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    // Main scoring method - call this when enemy dies
    public void ScoreEnemyKill(int enemyHealth)
    {
        // Calculate points based on health ranges
        int pointsEarned = CalculatePointsFromHealth(enemyHealth);
        
        // Store last kill info for display
        lastKillPoints = pointsEarned;
        lastKillHealth = enemyHealth;
        lastKillTime = Time.time;
        
        // Add to score
        AddScore(pointsEarned);
        
        // Increment kill count
        totalKills++;
        
        // Show visual feedback
        ShowKillFeedback(pointsEarned, enemyHealth);
        
        // Update watch display
        UpdateWatchDisplay();
        
        // Log for debugging
        Debug.Log($"Enemy killed! Health: {enemyHealth} → +{pointsEarned} points (Total: {currentScore}, Kills: {totalKills})");
    }
    
    // Calculate points based on health ranges
    int CalculatePointsFromHealth(int enemyHealth)
    {
        if (enemyHealth < healthThreshold1)
        {
            // Below 50 health - still give base score
            return baseScorePerKill;
        }
        else if (enemyHealth >= healthThreshold1 && enemyHealth < healthThreshold2)
        {
            // 50-99 health
            return 1;
        }
        else if (enemyHealth >= healthThreshold2 && enemyHealth < healthThreshold3)
        {
            // 100-149 health
            return 2;
        }
        else if (enemyHealth >= healthThreshold3 && enemyHealth < healthThreshold4)
        {
            // 150-199 health
            return 3;
        }
        else // enemyHealth >= healthThreshold4
        {
            // 200+ health
            return 4;
        }
    }
    
    // Alternative: More flexible calculation with adjustable thresholds
    public int CalculatePointsFromHealthAdvanced(int enemyHealth)
    {
        int points = baseScorePerKill; // Minimum 1 point
        
        // Check each threshold
        if (enemyHealth >= healthThreshold1) points = 1;
        if (enemyHealth >= healthThreshold2) points = 2;
        if (enemyHealth >= healthThreshold3) points = 3;
        if (enemyHealth >= healthThreshold4) points = 4;
        
        return points;
    }
    
    // Get color based on points earned
    Color GetPointsColor(int points)
    {
        switch (points)
        {
            case 4: return maxBonusColor;    // Red for max points
            case 3: return Color.magenta;    // Magenta for 3 points
            case 2: return Color.green;      // Green for 2 points
            case 1: return Color.cyan;       // Cyan for 1 point
            default: return normalScoreColor; // White for other
        }
    }
    
    // Get description of kill based on health
    string GetKillDescription(int health)
    {
        if (health < healthThreshold1)
            return "Weak Enemy";
        else if (health < healthThreshold2)
            return "Normal Enemy";
        else if (health < healthThreshold3)
            return "Strong Enemy";
        else if (health < healthThreshold4)
            return "Elite Enemy";
        else
            return "BOSS Enemy";
    }
    
    void ShowKillFeedback(int points, int health)
    {
        if (scoreText != null)
        {
            // Change color based on points
            Color pointsColor = GetPointsColor(points);
            scoreText.color = pointsColor;
            
            // Restore original color after delay
            if (bonusDisplayCoroutine != null)
                StopCoroutine(bonusDisplayCoroutine);
            
            bonusDisplayCoroutine = StartCoroutine(RestoreScoreColor());
        }
        
        // Optional: Show floating text at watch
        ShowPointsPopup(points, health);
    }
    
    IEnumerator RestoreScoreColor()
    {
        yield return new WaitForSeconds(bonusDisplayTime);
        
        if (scoreText != null)
            scoreText.color = originalScoreColor;
    }
    
    void ShowPointsPopup(int points, int health)
    {
        if (pointsPerKillText != null)
        {
            Color pointsColor = GetPointsColor(points);
            string enemyType = GetKillDescription(health);
            
            pointsPerKillText.text = $"+{points}\n<size=70%>{enemyType}\n{health} HP</size>";
            pointsPerKillText.color = pointsColor;
            
            // Hide after delay
            StartCoroutine(HidePointsPopup());
        }
    }
    
    IEnumerator HidePointsPopup()
    {
        yield return new WaitForSeconds(2f);
        
        if (pointsPerKillText != null)
        {
            pointsPerKillText.text = "";
        }
    }
    
    void AddScore(int points)
    {
        currentScore += points;
        
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }
    
    void UpdateWatchDisplay()
    {
        // Update score text
        if (scoreText != null)
            scoreText.text = $"SCORE: {currentScore}";
        
        // Update kills text
        if (killsText != null)
            killsText.text = $"KILLS: {totalKills}";
        
        // Update high score text
        if (highScoreText != null)
            highScoreText.text = $"HIGH: {highScore}";
        
        // Update points per kill text if available
        if (pointsPerKillText != null && Time.time - lastKillTime < 2f)
        {
            // Text is already set in ShowPointsPopup
            // Just update color if needed
            pointsPerKillText.color = GetPointsColor(lastKillPoints);
        }
    }
    
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("VRHighScore", highScore);
        PlayerPrefs.Save();
        Debug.Log($"New High Score! {highScore} points");
    }
    
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("VRHighScore", 0);
    }
    
    #region Public API
    
    public int GetCurrentScore() => currentScore;
    public int GetTotalKills() => totalKills;
    public int GetHighScore() => highScore;
    public int GetLastKillPoints() => lastKillPoints;
    public int GetLastKillHealth() => lastKillHealth;
    
    // Get current health thresholds
    public int[] GetHealthThresholds()
    {
        return new int[] { healthThreshold1, healthThreshold2, healthThreshold3, healthThreshold4 };
    }
    
    // Get points breakdown
    public string GetScoringRules()
    {
        return $"Scoring:\n" +
               $"<50 HP: {baseScorePerKill} point\n" +
               $"{healthThreshold1}-{healthThreshold2-1} HP: 1 point\n" +
               $"{healthThreshold2}-{healthThreshold3-1} HP: 2 points\n" +
               $"{healthThreshold3}-{healthThreshold4-1} HP: 3 points\n" +
               $">{healthThreshold4} HP: 4 points";
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        totalKills = 0;
        lastKillPoints = 0;
        lastKillHealth = 0;
        UpdateWatchDisplay();
        Debug.Log("Score reset to zero");
    }
    
    // Test the scoring system
    public void TestScoringSystem()
    {
        Debug.Log("=== Scoring System Test ===");
        Debug.Log(GetScoringRules());
        
        int[] testHealths = { 25, 50, 75, 100, 125, 150, 175, 200, 250, 300 };
        
        foreach (int health in testHealths)
        {
            int points = CalculatePointsFromHealth(health);
            Debug.Log($"{health} HP → {points} points ({GetKillDescription(health)})");
        }
    }
    
    // Adjust thresholds in-game
    public void SetHealthThresholds(int threshold1, int threshold2, int threshold3, int threshold4)
    {
        healthThreshold1 = Mathf.Max(1, threshold1);
        healthThreshold2 = Mathf.Max(healthThreshold1 + 1, threshold2);
        healthThreshold3 = Mathf.Max(healthThreshold2 + 1, threshold3);
        healthThreshold4 = Mathf.Max(healthThreshold3 + 1, threshold4);
        
        Debug.Log($"Health thresholds updated: {healthThreshold1}, {healthThreshold2}, {healthThreshold3}, {healthThreshold4}");
    }
    
    #endregion
    
    #region Debug/Editor
    
    void OnValidate()
    {
        // Ensure thresholds are in correct order
        healthThreshold1 = Mathf.Max(1, healthThreshold1);
        healthThreshold2 = Mathf.Max(healthThreshold1 + 1, healthThreshold2);
        healthThreshold3 = Mathf.Max(healthThreshold2 + 1, healthThreshold3);
        healthThreshold4 = Mathf.Max(healthThreshold3 + 1, healthThreshold4);
    }
    
    #if UNITY_EDITOR
    void Update()
    {
        // Debug controls in editor
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestScoringSystem();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Simulate killing enemies with different health values
            ScoreEnemyKill(Random.Range(30, 300));
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ResetScore();
        }
    }
    #endif
    
    #endregion
}