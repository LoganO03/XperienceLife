using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public float lifeTime = 0.1f;
    public float damage = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }
    }
}
