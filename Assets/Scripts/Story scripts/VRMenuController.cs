using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VRMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject quitConfirmPanel;
    [SerializeField] private GameObject debugPanel;
    
    [Header("Buttons - Main Menu")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button debugButton;
    
    [Header("Buttons - Options Menu")]
    [SerializeField] private Button backButton;
    
    [Header("Buttons - Quit Confirm Panel")]
    [SerializeField] private Button cancelButton;
    
    [Header("Buttons - Debug Panel")]
    [SerializeField] private Button debugBackButton;
    
    [Header("Turn Settings")]
    [SerializeField] private TMP_Dropdown turnDropdown;
    [SerializeField] private ActionBasedSnapTurnProvider snapTurn;
    [SerializeField] private ActionBasedContinuousTurnProvider continuousTurn;
    
    [Header("Menu Settings")]
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private float distanceFromHead = 1.5f;
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0.1f, 0);
    
    private Transform playerCamera;
    private Vector3 targetPosition;
    private bool isMenuOpen = false;
    
    // Controller input
    private InputDevice leftController;
    private bool lastYButtonState = false;
    private float menuCooldown = 0f;
    
    void Start()
    {
        playerCamera = Camera.main.transform;
        
        // Hide all menus at start
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);
        if (debugPanel != null)
            debugPanel.SetActive(false);
        
        // Setup button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(CloseMenu);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OpenQuitConfirm);
        
        if (debugButton != null)
            debugButton.onClick.AddListener(OpenDebug);
        
        if (backButton != null)
            backButton.onClick.AddListener(CloseOptions);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseQuitConfirm);
        
        if (debugBackButton != null)
            debugBackButton.onClick.AddListener(CloseDebug);
        
        // Setup turn dropdown
        if (turnDropdown != null)
        {
            turnDropdown.onValueChanged.AddListener(OnTurnTypeChanged);
            LoadCurrentTurnSetting();
        }
        
        // Find left controller
        FindLeftController();
    }
    
    void FindLeftController()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, devices);
        
        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log("Left controller found: " + leftController.name);
        }
        else
        {
            Debug.LogWarning("Left controller not found!");
        }
    }
    
    void Update()
    {
        // Update controller reference if needed
        if (!leftController.isValid)
        {
            FindLeftController();
        }
        
        // Check for Y button (secondary button on left controller)
        if (leftController.isValid)
        {
            if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool yButtonPressed))
            {
                // Only trigger on the frame the button is pressed (not held)
                if (yButtonPressed && !lastYButtonState && Time.time > menuCooldown)
                {
                    if (isMenuOpen)
                        CloseMenu();
                    else
                        OpenMenu();
                    
                    menuCooldown = Time.time + 0.3f; // Prevent multiple toggles
                }
                lastYButtonState = yButtonPressed;
            }
        }
        
        // Make menu follow camera when open
        if (isMenuOpen && mainMenuPanel != null && playerCamera != null)
        {
            targetPosition = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            mainMenuPanel.transform.position = Vector3.Lerp(mainMenuPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            // Face the camera
            Quaternion targetRotation = Quaternion.LookRotation(mainMenuPanel.transform.position - playerCamera.position);
            mainMenuPanel.transform.rotation = Quaternion.Slerp(mainMenuPanel.transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
        
        // Make options panel follow camera when open
        if (isMenuOpen && optionsPanel != null && optionsPanel.activeSelf && playerCamera != null)
        {
            targetPosition = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            optionsPanel.transform.position = Vector3.Lerp(optionsPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(optionsPanel.transform.position - playerCamera.position);
            optionsPanel.transform.rotation = Quaternion.Slerp(optionsPanel.transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
        
        // Make quit confirm panel follow camera when open
        if (isMenuOpen && quitConfirmPanel != null && quitConfirmPanel.activeSelf && playerCamera != null)
        {
            targetPosition = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            quitConfirmPanel.transform.position = Vector3.Lerp(quitConfirmPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(quitConfirmPanel.transform.position - playerCamera.position);
            quitConfirmPanel.transform.rotation = Quaternion.Slerp(quitConfirmPanel.transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
        
        // Make debug panel follow camera when open
        if (isMenuOpen && debugPanel != null && debugPanel.activeSelf && playerCamera != null)
        {
            targetPosition = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            debugPanel.transform.position = Vector3.Lerp(debugPanel.transform.position, targetPosition, followSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(debugPanel.transform.position - playerCamera.position);
            debugPanel.transform.rotation = Quaternion.Slerp(debugPanel.transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
        
        // DEBUG: Press Space to force menu open/close (for testing)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }
    
    void OpenMenu()
    {
        isMenuOpen = true;
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        debugPanel.SetActive(false);
        
        // Position menu in front of camera
        if (playerCamera != null)
        {
            mainMenuPanel.transform.position = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            mainMenuPanel.transform.LookAt(playerCamera);
            mainMenuPanel.transform.Rotate(0, 180, 0);
        }
        
        Debug.Log("Menu opened");
    }
    
    void CloseMenu()
    {
        isMenuOpen = false;
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        debugPanel.SetActive(false);
        Debug.Log("Menu closed");
    }
    
    void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
        debugPanel.SetActive(false);
        Debug.Log("Options opened");
    }
    
    void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Options closed");
    }
    
    void OpenQuitConfirm()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
        debugPanel.SetActive(false);
        Debug.Log("Quit confirm panel opened");
    }
    
    void CloseQuitConfirm()
    {
        quitConfirmPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Quit confirm panel closed - returned to main menu");
    }
    
    void OpenDebug()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
        debugPanel.SetActive(true);
        Debug.Log("Debug panel opened");
    }
    
    void CloseDebug()
    {
        debugPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Debug.Log("Debug panel closed - returned to main menu");
    }
    
    void OnTurnTypeChanged(int value)
    {
        PlayerPrefs.SetInt("turn", value);
        ApplyTurnSettings(value);
    }
    
    void ApplyTurnSettings(int value)
    {
        // Enable ONE, disable the OTHER
        if (value == 0) // Snap Turn
        {
            if (snapTurn != null)
                snapTurn.enabled = true;
            if (continuousTurn != null)
                continuousTurn.enabled = false;
            Debug.Log("Snap Turn enabled");
        }
        else // Continuous Turn
        {
            if (snapTurn != null)
                snapTurn.enabled = false;
            if (continuousTurn != null)
                continuousTurn.enabled = true;
            Debug.Log("Continuous Turn enabled");
        }
    }
    
    void LoadCurrentTurnSetting()
    {
        if (PlayerPrefs.HasKey("turn"))
        {
            int savedValue = PlayerPrefs.GetInt("turn");
            if (turnDropdown != null)
                turnDropdown.SetValueWithoutNotify(savedValue);
            ApplyTurnSettings(savedValue);
        }
        else
        {
            if (turnDropdown != null)
                turnDropdown.SetValueWithoutNotify(0);
            ApplyTurnSettings(0);
        }
    }
    
    void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(CloseMenu);
        
        if (optionsButton != null)
            optionsButton.onClick.RemoveListener(OpenOptions);
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OpenQuitConfirm);
        
        if (debugButton != null)
            debugButton.onClick.RemoveListener(OpenDebug);
        
        if (backButton != null)
            backButton.onClick.RemoveListener(CloseOptions);
        
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(CloseQuitConfirm);
        
        if (debugBackButton != null)
            debugBackButton.onClick.RemoveListener(CloseDebug);
        
        if (turnDropdown != null)
            turnDropdown.onValueChanged.RemoveListener(OnTurnTypeChanged);
    }
}