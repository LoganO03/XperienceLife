using UnityEngine;

public class EnemyMeleeHitbox : MonoBehaviour
{
    public float lifeTime = 0.1f;
    public float damage = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage PLAYER
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }
    }
}
