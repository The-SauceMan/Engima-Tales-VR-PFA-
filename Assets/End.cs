using UnityEngine;

public class End : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private float distanceFromHead = 1.5f;
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0.1f, 0);
    
    private Transform playerCamera;
    private Vector3 targetPosition;
    
    void Start()
    {
        playerCamera = Camera.main.transform;
    }
    
    void Update()
    {
        if (playerCamera == null) return;
        
        // Calculate target position (same as VR menu)
        targetPosition = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
        
        // Smoothly move to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Face the camera (same as VR menu)
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
    
    // Call this to instantly position the UI (like when opening menu)
    public void SnapToCamera()
    {
        if (playerCamera != null)
        {
            transform.position = playerCamera.position + (playerCamera.forward * distanceFromHead) + positionOffset;
            transform.LookAt(playerCamera);
            transform.Rotate(0, 180, 0);
        }
    }
}