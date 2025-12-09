using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 6f;
    public float damage;
    private Vector2 direction;

    public GameObject explosionPrefab;

    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;

        // 🔥 Rotate the projectile to face the direction it's moving
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage enemies
        if (other.CompareTag("Enemy"))
        {
            if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            // Optional: die on hitting walls
            Destroy(gameObject);
        }
    }
}
