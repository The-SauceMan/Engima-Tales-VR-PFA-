using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BookTeleportUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button teleportButton;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    [Header("Book Reference")]
    [SerializeField] private XRGrabInteractable book;
    
    [Header("Smooth Following")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float fixedDistance = 1.5f;
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0.2f, 0);
    
    private Transform playerCamera;
    private Vector3 targetPosition;
    private bool isUIActive = false;
    private bool isTeleporting = false;
    
    void Start()
    {
        playerCamera = Camera.main.transform;
        
        // Setup UI
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            
            // Setup button
            if (teleportButton != null)
            {
                teleportButton.onClick.AddListener(OnTeleportButtonPressed);
            }
        }
        
        // Subscribe to book grab events
        if (book != null)
        {
            book.selectEntered.AddListener(OnBookGrabbed);
            book.selectExited.AddListener(OnBookReleased);
        }
    }
    
    void Update()
    {
        if (isUIActive && uiPanel != null && playerCamera != null && !isTeleporting)
        {
            // Smoothly follow camera
            targetPosition = playerCamera.position + (playerCamera.forward * fixedDistance) + positionOffset;
            uiPanel.transform.position = Vector3.Lerp(uiPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            // Face camera
            Quaternion targetRotation = Quaternion.LookRotation(uiPanel.transform.position - playerCamera.position);
            uiPanel.transform.rotation = Quaternion.Slerp(uiPanel.transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }
    
    void OnBookGrabbed(SelectEnterEventArgs args)
    {
        if (uiPanel != null && !isTeleporting)
        {
            uiPanel.SetActive(true);
            isUIActive = true;
            
            // Initial position
            if (playerCamera != null)
            {
                uiPanel.transform.position = playerCamera.position + (playerCamera.forward * fixedDistance) + positionOffset;
                uiPanel.transform.LookAt(playerCamera);
                uiPanel.transform.Rotate(0, 180, 0);
            }
        }
    }
    
    void OnBookReleased(SelectExitEventArgs args)
    {
        if (uiPanel != null && !isTeleporting)
        {
            uiPanel.SetActive(false);
            isUIActive = false;
        }
    }
    
    void OnTeleportButtonPressed()
    {
        if (isTeleporting) return;
        
        StartCoroutine(TeleportSequence());
    }
    
    IEnumerator TeleportSequence()
    {
        isTeleporting = true;
        
        // Change button text
        if (buttonText != null)
            buttonText.text = "Teleporting...";
        
        // Disable button to prevent multiple presses
        if (teleportButton != null)
            teleportButton.interactable = false;
        
        // Small delay for visual feedback
        yield return new WaitForSeconds(0.5f);
        
        // Hide the book UI immediately
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            isUIActive = false;
        }
        
        // TODO: Add your teleport logic here
        Debug.Log("Teleport to scene would happen here!");
    }
    
    void OnDestroy()
    {
        if (book != null)
        {
            book.selectEntered.RemoveListener(OnBookGrabbed);
            book.selectExited.RemoveListener(OnBookReleased);
        }
        
        if (teleportButton != null)
            teleportButton.onClick.RemoveListener(OnTeleportButtonPressed);
    }
}