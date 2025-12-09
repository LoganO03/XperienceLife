using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeHitboxPrefab; // prefab with EnemyMeleeHitbox + collider
    [SerializeField] private Transform player;             // can be left null; auto-found via tag

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.4f;     // distance between colliders before attacking
    [SerializeField] private float innerOffset = 0.1f;     // extra offset beyond collider edge
    [SerializeField] private float hitboxDuration = 0.1f;  // how long the hitbox exists
    [SerializeField] private float attackCooldown = 1.0f;  // delay between attacks
    [SerializeField] private float damage = 1f;               // damage dealt to player

    private bool canAttack = true;

    private EnemyMovement enemyMovement;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyCollider = GetComponent<Collider2D>();   // zombie collider

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player != null)
            playerCollider = player.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (player == null) return;

        float dist;

        // Prefer accurate collider-vs-collider distance so tall / wide shapes work
        if (enemyCollider != null && playerCollider != null)
        {
            var d = Physics2D.Distance(enemyCollider, playerCollider);
            dist = d.distance; // shortest distance between the two shapes
        }
        else
        {
            // Fallback to center distance if something is missing
            dist = Vector2.Distance(transform.position, player.position);
        }

        // Only attack if within range and off cooldown
        if (dist <= attackRange && canAttack)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (!canAttack || meleeHitboxPrefab == null || player == null)
            return;

        // Start the attack animation immediately
        if (enemyMovement != null)
        {
            enemyMovement.StartAttackAnimation();
        }

        // Direction from enemy to player
        Vector2 dir = (player.position - transform.position).normalized;

        // Spawn hitbox at the edge of the enemy collider, in the direction of the player
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

        // Rotate triangle so its tip points toward the player
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        // Configure the hitbox
        var hb = hitboxObj.GetComponent<EnemyMeleeHitbox>();
        if (hb != null)
        {
            hb.lifeTime = hitboxDuration;
            hb.damage = damage;
        }

        // Start cooldown so it doesn't spam attacks every frame
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
