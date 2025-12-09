using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public float lifeTime = 0.1f;
    public float baseDamage = 1f;
    public float bonusDamage = 0f; // we’ll set this from PlayerStats

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private float TotalDamage => baseDamage + bonusDamage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth hp = other.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(TotalDamage);
            }
        }
    }
}
