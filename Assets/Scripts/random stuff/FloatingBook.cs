using UnityEngine;

public class FloatingBook : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.1f;    // How high/low it moves
    [SerializeField] private float floatSpeed = 1.5f;        // Speed of the bobbing motion
    [SerializeField] private float rotationAmplitude = 10f;  // How much it tilts (degrees)
    [SerializeField] private float rotationSpeed = 1.2f;     // Speed of the tilt motion
    [SerializeField] private bool randomizeStartOffset = true; // Random starting position
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startOffset;
    
    void Start()
    {
        // Store the initial position and rotation
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        // Set random start offset for variety
        if (randomizeStartOffset)
        {
            startOffset = Random.Range(0f, Mathf.PI * 2f);
        }
    }
    
    void Update()
    {
        // Calculate the vertical bobbing motion using sine wave
        float floatY = Mathf.Sin((Time.time + startOffset) * floatSpeed) * floatAmplitude;
        
        // Apply the new position
        transform.position = startPosition + new Vector3(0f, floatY, 0f);
        
        // Calculate the tilt based on the direction of movement
        float floatDirection = Mathf.Cos((Time.time + startOffset) * floatSpeed);
        float tiltZ = floatDirection * rotationAmplitude;
        
        // Add a subtle rocking motion side-to-side
        float tiltX = Mathf.Sin((Time.time + startOffset) * rotationSpeed) * (rotationAmplitude * 0.3f);
        
        // Apply the rotation
        transform.rotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);
    }
    
    // Visualize the float range in the editor
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