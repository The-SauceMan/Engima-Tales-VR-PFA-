using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.2f;    // How high/low it moves
    [SerializeField] private float floatSpeed = 1.5f;        // Speed of the bobbing motion
    [SerializeField] private float rotationAmplitude = 15f;  // How much it tilts (degrees)
    [SerializeField] private float rotationSpeed = 1.2f;     // Speed of the tilt motion
    
    [Header("Offset Settings")]
    [SerializeField] private bool randomizeStartOffset = true; // Random starting position in the cycle
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startOffset;
    
    void Start()
    {
        // Store the initial position and rotation
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        // Set random start offset for variety if multiple arrows exist
        if (randomizeStartOffset)
        {
            startOffset = Random.Range(0f, Mathf.PI * 2f);
        }
        else
        {
            startOffset = 0f;
        }
    }
    
    void Update()
    {
        // Calculate the vertical bobbing motion using sine wave
        float floatY = Mathf.Sin((Time.time + startOffset) * floatSpeed) * floatAmplitude;
        
        // Apply the new position
        transform.position = startPosition + new Vector3(0f, floatY, 0f);
        
        // Calculate the tilt based on the direction of movement
        // This makes the arrow tilt slightly up when going up, down when going down
        float floatDirection = Mathf.Cos((Time.time + startOffset) * floatSpeed);
        float tiltZ = floatDirection * rotationAmplitude;
        
        // Optional: Add a subtle rocking motion side-to-side
        float tiltX = Mathf.Sin((Time.time + startOffset) * rotationSpeed) * (rotationAmplitude * 0.3f);
        
        // Apply the rotation (adjust axes based on your arrow's orientation)
        // For an arrow pointing forward (Z-axis is forward):
        transform.rotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
        
        // Alternative: If your arrow uses different axes, uncomment one of these:
        // For an arrow pointing right (X-axis is forward):
        // transform.rotation = startRotation * Quaternion.Euler(0f, 0f, tiltZ);
        // For an arrow pointing up (Y-axis is forward):
        // transform.rotation = startRotation * Quaternion.Euler(tiltZ, 0f, 0f);
    }
    
    // Optional: Visualize the float range in the editor
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(startPosition, new Vector3(0.5f, floatAmplitude * 2f, 0.5f));
        }
        else if (!Application.isPlaying && transform != null)
        {
            // Show preview in editor
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Vector3 previewPos = transform.position;
            Vector3 bottom = previewPos - Vector3.up * floatAmplitude;
            Vector3 top = previewPos + Vector3.up * floatAmplitude;
            Gizmos.DrawLine(bottom, top);
            Gizmos.DrawWireSphere(bottom, 0.05f);
            Gizmos.DrawWireSphere(top, 0.05f);
        }
    }
}