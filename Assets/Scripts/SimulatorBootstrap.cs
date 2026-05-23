using UnityEngine;

public class SimulatorBootstrap : MonoBehaviour
{
#if !UNITY_EDITOR
    void Awake()
    {
        // Destroy the simulator when running on a real device
        Destroy(gameObject);
    }
#endif
}