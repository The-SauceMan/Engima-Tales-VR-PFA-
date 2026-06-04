using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using System.Collections;

public class SkipUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private GameObject skipUIPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private float skipToTime = 10f;
    
    [Header("Follow Settings")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private float followSpeed = 8f;
    
    [Header("Face Settings")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private Camera targetCamera;
    
    private CanvasGroup canvasGroup;
    private bool isVisible = true; // Start visible
    
    void Start()
    {
        // Auto-setup references
        if (timelineDirector == null)
            timelineDirector = FindObjectOfType<PlayableDirector>();
        
        if (followTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                followTarget = player.transform;
            else
                followTarget = Camera.main.transform;
        }
        
        if (targetCamera == null)
            targetCamera = Camera.main;
        
        if (skipUIPanel != null)
        {
            canvasGroup = skipUIPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = skipUIPanel.AddComponent<CanvasGroup>();
            
            // MAKE SURE UI IS ACTIVE AT START
            skipUIPanel.SetActive(true);
            canvasGroup.alpha = 1;
        }
        
        // Setup buttons
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmSkip);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSkip);
        
        Debug.Log("Skip UI initialized and visible");
    }
    
    void Update()
    {
        if (!isVisible || skipUIPanel == null || followTarget == null)
            return;
        
        // Follow the target
        Vector3 targetPosition = followTarget.position + offset;
        
        if (followSpeed > 0)
            skipUIPanel.transform.position = Vector3.Lerp(skipUIPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
        else
            skipUIPanel.transform.position = targetPosition;
        
        // Face the camera
        if (faceCamera && targetCamera != null)
        {
            Vector3 direction = skipUIPanel.transform.position - targetCamera.transform.position;
            skipUIPanel.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    private void ConfirmSkip()
    {
        if (timelineDirector != null)
        {
            bool wasPlaying = timelineDirector.state == PlayState.Playing;
            timelineDirector.Stop();
            timelineDirector.time = skipToTime;
            if (wasPlaying)
                timelineDirector.Play();
            
            Debug.Log($"Skipped to {skipToTime} seconds");
        }
        
        // Hide UI when confirm is pressed
        HideSkipUI();
    }
    
    private void CancelSkip()
    {
        Debug.Log("Skip cancelled");
        // Hide UI when cancel is pressed
        HideSkipUI();
    }
    
    private void HideSkipUI()
    {
        if (skipUIPanel == null) return;
        isVisible = false;
        skipUIPanel.SetActive(false);
        Debug.Log("Skip UI hidden");
    }
    
    // Optional: Public method to show again if needed
    public void ShowSkipUI()
    {
        if (skipUIPanel == null) return;
        isVisible = true;
        skipUIPanel.SetActive(true);
        Debug.Log("Skip UI shown");
    }
    
    void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(ConfirmSkip);
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(CancelSkip);
    }
}