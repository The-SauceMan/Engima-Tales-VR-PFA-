using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] public float damageAmount = 25f;
    
    [Header("Destruction")]
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float destroyDelay = 0.1f;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turtle"))
        {
            TurtleStamina turtle = other.GetComponent<TurtleStamina>();
            if (turtle != null)
            {
                turtle.TakeDamage(damageAmount);
            }
            
            if (destroyOnHit)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}