using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;  // The UI panel that appears on screen
    [SerializeField] private XRGrabInteractable targetObject;  // The object to detect grab on
    
    [Header("Optional - Multiple Objects")]
    [SerializeField] private XRGrabInteractable[] additionalObjects;  // Multiple objects that show same UI
    
    [Header("UI Behavior")]
    [SerializeField] private bool showOnGrab = true;
    [SerializeField] private bool hideOnRelease = true;
    [SerializeField] private float showDelay = 0f;  // Optional delay before showing
    [SerializeField] private float hideDelay = 0f;  // Optional delay before hiding
    
    private bool isUIVisible = false;
    private Coroutine currentCoroutine;
    
    void Start()
    {
        // Ensure UI starts hidden
        if (uiPanel != null)
            uiPanel.SetActive(false);
        
        // Subscribe to grab events on main object
        if (targetObject != null)
        {
            targetObject.selectEntered.AddListener(OnObjectGrabbed);
            targetObject.selectExited.AddListener(OnObjectReleased);
        }
        
        // Subscribe to additional objects if any
        foreach (var obj in additionalObjects)
        {
            if (obj != null)
            {
                obj.selectEntered.AddListener(OnObjectGrabbed);
                obj.selectExited.AddListener(OnObjectReleased);
            }
        }
    }
    
    void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        if (showOnGrab)
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            
            if (showDelay > 0)
                currentCoroutine = StartCoroutine(ShowUIAfterDelay(showDelay));
            else
                ShowUI();
        }
    }
    
    void OnObjectReleased(SelectExitEventArgs args)
    {
        if (hideOnRelease)
        {
            // Check if any other objects are still being grabbed
            bool anyOtherGrabbed = false;
            
            if (targetObject != null && targetObject.isSelected && args.interactableObject != targetObject)
                anyOtherGrabbed = true;
            
            foreach (var obj in additionalObjects)
            {
                if (obj != null && obj.isSelected && args.interactableObject != obj)
                {
                    anyOtherGrabbed = true;
                    break;
                }
            }
            
            // Only hide if nothing else is grabbed
            if (!anyOtherGrabbed)
            {
                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
                
                if (hideDelay > 0)
                    currentCoroutine = StartCoroutine(HideUIAfterDelay(hideDelay));
                else
                    HideUI();
            }
        }
    }
    
    void ShowUI()
    {
        if (uiPanel != null && !isUIVisible)
        {
            uiPanel.SetActive(true);
            isUIVisible = true;
            Debug.Log($"UI shown - {targetObject.name} is grabbed");
        }
    }
    
    void HideUI()
    {
        if (uiPanel != null && isUIVisible)
        {
            uiPanel.SetActive(false);
            isUIVisible = false;
            Debug.Log("UI hidden - object released");
        }
    }
    
    System.Collections.IEnumerator ShowUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowUI();
    }
    
    System.Collections.IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideUI();
    }
    
    // Public method to manually control UI
    public void ForceShowUI()
    {
        ShowUI();
    }
    
    public void ForceHideUI()
    {
        HideUI();
    }
    
    void OnDestroy()
    {
        // Clean up event listeners
        if (targetObject != null)
        {
            targetObject.selectEntered.RemoveListener(OnObjectGrabbed);
            targetObject.selectExited.RemoveListener(OnObjectReleased);
        }
        
        foreach (var obj in additionalObjects)
        {
            if (obj != null)
            {
                obj.selectEntered.RemoveListener(OnObjectGrabbed);
                obj.selectExited.RemoveListener(OnObjectReleased);
            }
        }
    }
}