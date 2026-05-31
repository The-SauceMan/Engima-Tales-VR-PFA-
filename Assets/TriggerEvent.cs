using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyOnTrigger = false;
    [SerializeField] private float destroyDelay = 0f;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onPlayerEnter; // Event when player enters trigger
    [SerializeField] private UnityEvent onPlayerExit;  // Event when player exits trigger
    
    private bool hasTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Trigger the event
            if (onPlayerEnter != null)
            {
                onPlayerEnter.Invoke();
            }
            
            Debug.Log($"Player entered trigger: {gameObject.name}");
            
            // Destroy if enabled
            if (destroyOnTrigger)
            {
                if (destroyDelay > 0)
                    Destroy(gameObject, destroyDelay);
                else
                    Destroy(gameObject);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Trigger exit event
            if (onPlayerExit != null)
            {
                onPlayerExit.Invoke();
            }
            
            Debug.Log($"Player exited trigger: {gameObject.name}");
        }
    }
}