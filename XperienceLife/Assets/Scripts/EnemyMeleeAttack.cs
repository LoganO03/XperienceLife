using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject meleeHitboxPrefab; // EnemyMeleeHitbox prefab
    [SerializeField] private Transform player;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float innerOffset = 0.3f;      // how far from enemy the tip appears
    [SerializeField] private float hitboxDuration = 0.1f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int damage = 1;

    private bool canAttack = true;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange && canAttack)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (!canAttack || meleeHitboxPrefab == null || player == null)
            return;

        Vector2 dir = (player.position - transform.position).normalized;

        // Spawn tip slightly in front of enemy
        Vector3 spawnPos = transform.position + (Vector3)(dir * innerOffset);
        GameObject hitboxObj = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

        // Triangle tip is at top, so like player: local forward is -Y from tip
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        var hb = hitboxObj.GetComponent<EnemyMeleeHitbox>();
        if (hb != null){
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