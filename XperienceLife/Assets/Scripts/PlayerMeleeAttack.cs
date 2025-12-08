using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private PlayerStats playerStats;

    [Header("Attack Settings")]
    [SerializeField] private float hitboxDuration = 0.1f;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private float innerOffset = 0.3f; // how far away from the player the tip starts





    private bool canAttack = true;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();
    }

    public void PerformAttack()
    {
        if (!canAttack || meleeHitboxPrefab == null || playerMovement == null)
            return;

        Vector2 dir = playerMovement.AimDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        // Put the tip slightly in front of the player
        Vector3 spawnPos = playerMovement.transform.position + (Vector3)(dir * innerOffset);

        GameObject hitboxObj = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

        // Our triangle's "forward" (from tip to base) is DOWN (-Y),
        // because the tip is at the top of the sprite.
        // So we map local (0, -1) to AimDirection => angle + 90.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        var hb = hitboxObj.GetComponent<MeleeHitbox>();
        if (hb != null)
            hb.lifeTime = hitboxDuration;

            if (playerStats != null)
                hb.bonusDamage = playerStats.strength;
                
        StartCoroutine(Cooldown());
    }



    private IEnumerator Cooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
