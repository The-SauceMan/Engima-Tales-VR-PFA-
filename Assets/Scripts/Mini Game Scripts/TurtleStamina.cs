using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class TurtleStamina : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private TextMeshProUGUI staminaText;
    
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;
    
    [Header("Colors")]
    [SerializeField] private Color fullStaminaColor = Color.blue;
    [SerializeField] private Color damagedStaminaColor = Color.yellow;
    [SerializeField] private Color criticalStaminaColor = Color.red;
    [SerializeField] private float criticalThreshold = 0.25f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Stamina Depleted Event")]
    [SerializeField] private UnityEvent onStaminaDepleted;
    [SerializeField] private float eventDelaySeconds = 0f;
    [SerializeField] private AudioClip staminaDepletedSound;
    [SerializeField] private float staminaDepletedVolume = 1f; // Volume for depleted sound
    
    [Header("UI Offset")]
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 1.2f, 0);
    
    private GameObject staminaBarObject;
    private float lastStamina = -1;
    private bool isStaminaDepletedEventTriggered = false;
    
    void Start()
    {
        // Setup audio
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Setup UI parent for billboarding
        if (staminaSlider != null)
        {
            staminaBarObject = staminaSlider.transform.parent.gameObject;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
            staminaBarObject.transform.position = transform.position + uiOffset;
            staminaBarObject.transform.SetParent(transform);
        }
        
        UpdateUI();
    }
    
    void Update()
    {
        // Update UI only when stamina changes
        if (lastStamina != currentStamina)
        {
            UpdateUI();
        }
        
        // Make UI face camera
        if (staminaBarObject != null && Camera.main != null)
        {
            staminaBarObject.transform.LookAt(staminaBarObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                               Camera.main.transform.rotation * Vector3.up);
        }
    }
    
    public void TakeDamage(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
        
        UpdateUI();
        
        // Play random hit sound
        PlayRandomHitSound();
        
        // Check if stamina reached 0 and event hasn't been triggered yet
        if (currentStamina <= 0 && !isStaminaDepletedEventTriggered)
        {
            OnStaminaDepleted();
        }
    }
    
    void PlayRandomHitSound()
    {
        if (hitSounds != null && hitSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            AudioClip randomSound = hitSounds[randomIndex];
            audioSource.PlayOneShot(randomSound);
        }
    }
    
    public void AddStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        UpdateUI();
        
        // Reset the depleted event flag if stamina goes above 0
        if (currentStamina > 0)
        {
            isStaminaDepletedEventTriggered = false;
        }
    }
    
    void UpdateUI()
    {
        float staminaPercentage = currentStamina / maxStamina;
        lastStamina = currentStamina;
        
        // Update slider
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        
        // Update fill color based on stamina percentage
        if (staminaFillImage != null)
        {
            if (staminaPercentage <= criticalThreshold)
                staminaFillImage.color = criticalStaminaColor;
            else if (staminaPercentage <= 0.5f)
                staminaFillImage.color = damagedStaminaColor;
            else
                staminaFillImage.color = fullStaminaColor;
        }
        
        // Update text
        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.RoundToInt(currentStamina)} / {Mathf.RoundToInt(maxStamina)}";
        }
    }
    
    void OnStaminaDepleted()
    {
        isStaminaDepletedEventTriggered = true;
        
        // Play stamina depleted sound with custom volume
        if (staminaDepletedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(staminaDepletedSound, staminaDepletedVolume);
        }
        
        // Invoke the event with delay
        if (onStaminaDepleted != null)
        {
            if (eventDelaySeconds > 0)
                Invoke(nameof(TriggerEvent), eventDelaySeconds);
            else
                TriggerEvent();
        }
    }
    
    void TriggerEvent()
    {
        onStaminaDepleted.Invoke();
    }
    
    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public float GetStaminaPercentage() => currentStamina / maxStamina;
    
    void OnDestroy()
    {
        if (staminaBarObject != null)
            Destroy(staminaBarObject);
    }
}