using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;

public class HoverTriggerToggleUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Vector3 uiOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private bool followObject = true;
    
    [Header("Optional")]
    [SerializeField] private string displayText = "";
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Button cancelButton; // Reference to cancel button
    
    private XRSimpleInteractable interactable;
    private GameObject spawnedUI;
    private bool isUIOpen = false;
    private bool isHovering = false;
    
    void Start()
    {
        // Get or add interactable component
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        
        // Subscribe to hover events
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        
        // Subscribe to select events (trigger press)
        interactable.selectEntered.AddListener(OnTriggerPressed);
        interactable.selectExited.AddListener(OnTriggerReleased);
        
        // Hide UI at start
        if (uiPanel != null)
            uiPanel.SetActive(false);
    }
    
    void Update()
    {
        // Update UI position if following object and UI is open
        if (followObject && isUIOpen && spawnedUI != null)
        {
            spawnedUI.transform.position = transform.position + uiOffset;
            
            // Face camera (billboard effect)
            if (Camera.main != null)
            {
                spawnedUI.transform.LookAt(spawnedUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                          Camera.main.transform.rotation * Vector3.up);
            }
        }
    }
    
    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovering = true;
        Debug.Log($"Hovering over: {gameObject.name}");
    }
    
    private void OnHoverExit(HoverExitEventArgs args)
    {
        isHovering = false;
    }
    
    private void OnTriggerPressed(SelectEnterEventArgs args)
    {
        // Only toggle if we're hovering over the object
        if (isHovering)
        {
            if (isUIOpen)
                HideUI();
            else
                ShowUI();
        }
    }
    
    private void OnTriggerReleased(SelectExitEventArgs args)
    {
        // Trigger released - nothing needed here for toggle
    }
    
    void ShowUI()
    {
        if (uiPanel != null)
        {
            if (followObject)
            {
                // Create UI at object position
                spawnedUI = Instantiate(uiPanel, transform.position + uiOffset, Quaternion.identity);
                spawnedUI.transform.SetParent(transform);
                
                // Find cancel button in the spawned UI
                Button foundButton = spawnedUI.GetComponentInChildren<Button>();
                if (foundButton != null)
                {
                    foundButton.onClick.AddListener(OnCancelButtonPressed);
                    cancelButton = foundButton;
                }
                
                // Face camera
                if (Camera.main != null)
                {
                    spawnedUI.transform.LookAt(spawnedUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
                                              Camera.main.transform.rotation * Vector3.up);
                }
            }
            else
            {
                uiPanel.SetActive(true);
                spawnedUI = uiPanel;
                
                // Find cancel button
                if (cancelButton == null)
                {
                    cancelButton = spawnedUI.GetComponentInChildren<Button>();
                    if (cancelButton != null)
                        cancelButton.onClick.AddListener(OnCancelButtonPressed);
                }
            }
            
            // Set text if needed
            if (textComponent != null && !string.IsNullOrEmpty(displayText))
            {
                // Find text component if it's in the spawned UI
                TextMeshProUGUI foundText = spawnedUI.GetComponentInChildren<TextMeshProUGUI>();
                if (foundText != null)
                    foundText.text = displayText;
            }
            
            isUIOpen = true;
            Debug.Log($"UI opened for: {gameObject.name}");
        }
    }
    
    void HideUI()
    {
        if (followObject && spawnedUI != null)
        {
            // Remove button listener before destroying
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelButtonPressed);
            
            Destroy(spawnedUI);
        }
        else if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        
        isUIOpen = false;
        Debug.Log($"UI closed for: {gameObject.name}");
    }
    
    void OnCancelButtonPressed()
    {
        Debug.Log("Cancel button pressed - closing UI");
        HideUI();
    }
    
    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnTriggerPressed);
            interactable.selectExited.RemoveListener(OnTriggerReleased);
        }
        
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(OnCancelButtonPressed);
    }
}