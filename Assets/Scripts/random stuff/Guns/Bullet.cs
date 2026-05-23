using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 25;

    void OnCollisionEnter(Collision collision)
    {
        // Only deal damage if it hits an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyFunction enemy = collision.gameObject.GetComponent<EnemyFunction>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); // destroy bullet only when hitting Enemy
        }
    }
}
