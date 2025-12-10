using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private Transform player;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.4f;
    [SerializeField] private float innerOffset = 0.1f;
    [SerializeField] private float hitboxDuration = 0.1f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float damage = 1f;      // per-enemy base damage in inspector

    private float baseDamage;                        // stored original

    private bool canAttack = true;

    private EnemyMovement enemyMovement;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyCollider = GetComponent<Collider2D>();

        // remember original damage for difficulty scaling
        baseDamage = damage;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player != null)
            playerCollider = player.GetComponent<Collider2D>();
    }

    /// <summary>
    /// Called by spawner after Instantiate to apply wave-based difficulty.
    /// </summary>
    public void ApplyDifficultyBonus(int bonus)
    {
        damage = baseDamage + bonus;
    }

    private void Update()
    {
        if (player == null) return;

        float dist;

        if (enemyCollider != null && playerCollider != null)
        {
            var d = Physics2D.Distance(enemyCollider, playerCollider);
            dist = d.distance;
        }
        else
        {
            dist = Vector2.Distance(transform.position, player.position);
        }

        if (dist <= attackRange && canAttack)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (!canAttack || meleeHitboxPrefab == null || player == null)
            return;

        if (enemyMovement != null)
        {
            enemyMovement.StartAttackAnimation();
        }

        Vector2 dir = (player.position - transform.position).normalized;

        Vector3 center = transform.position;
        float radius = 0f;

        if (enemyCollider != null)
        {
            center = enemyCollider.bounds.center;
            Vector3 extents = enemyCollider.bounds.extents;
            radius = Mathf.Max(extents.x, extents.y);
        }

        Vector3 spawnPos = center + (Vector3)(dir * (radius + innerOffset));
        GameObject hitboxObj = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        var hb = hitboxObj.GetComponent<EnemyMeleeHitbox>();
        if (hb != null)
        {
            hb.lifeTime = hitboxDuration;
            hb.damage = damage;
        }

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
