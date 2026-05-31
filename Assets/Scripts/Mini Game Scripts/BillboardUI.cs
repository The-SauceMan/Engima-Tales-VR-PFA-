using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cameraTransform;
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    
    void LateUpdate()
    {
        // Make UI always face the camera
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                         cameraTransform.rotation * Vector3.up);
    }
}