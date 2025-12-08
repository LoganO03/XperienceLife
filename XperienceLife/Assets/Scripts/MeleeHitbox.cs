using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public float lifeTime = 0.1f;
    public int baseDamage = 1;
    public int bonusDamage = 0; // we’ll set this from PlayerStats

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private int TotalDamage => baseDamage + bonusDamage;

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
