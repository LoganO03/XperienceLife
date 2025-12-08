using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 2;

    private Vector2 direction;

    public void Initialize(Vector2 dir, int bonusDamage)
    {
        direction = dir.normalized;
        damage += bonusDamage; // scale by magic or strength if you want
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
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
