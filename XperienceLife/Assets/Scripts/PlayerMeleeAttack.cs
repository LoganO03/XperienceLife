using System.Collections;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject meleeHitboxPrefab;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerAnimationController animController;

    [Header("Attack Settings")]
    [SerializeField] private float hitboxDuration = 0.1f;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private float innerOffset = 0.3f; // how far away from the player the tip starts

    private bool canAttack = true;

    // direction we’ll use when the animation reaches the hit frame
    private Vector2 queuedAttackDir = Vector2.right;
    private bool hasQueuedAttack = false;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (animController == null)
            animController = GetComponentInChildren<PlayerAnimationController>();
    }

    /// <summary>
    /// Called from input. Starts the attack animation and queues data,
    /// but does NOT spawn the hitbox yet.
    /// </summary>
    public void PerformAttack()
    {
        Debug.Log("[Melee] PerformAttack (request) called");

        // hard guard: don't attack while casting
        if (animController != null && animController.IsCasting)
        {
            Debug.Log("[Melee] Blocked attack because player is casting");
            return;
        }

        if (!canAttack || meleeHitboxPrefab == null || playerMovement == null)
            return;

        Vector2 dir = playerMovement.AimDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        queuedAttackDir = dir.normalized;
        hasQueuedAttack = true;

        // play attack animation
        if (animController != null)
        {
            Debug.Log("[Melee] PlayAttack()");
            animController.PlayAttack();
        }

        // start cooldown as soon as attack starts
        StartCoroutine(Cooldown());
    }

    /// <summary>
    /// Animation Event – call this from the PAttack clip
    /// at the frame where the weapon should hit.
    /// </summary>
    public void Animation_MeleeHit()
    {
        Debug.Log("[Melee] Animation_MeleeHit event fired");

        if (!hasQueuedAttack)
        {
            // no queued data – nothing to do
            return;
        }
        hasQueuedAttack = false;

        if (meleeHitboxPrefab == null || playerMovement == null)
            return;

        Vector2 dir = queuedAttackDir;
        if (dir.sqrMagnitude < 0.001f)
            dir = playerMovement.AimDirection.normalized;

        if (dir.sqrMagnitude < 0.001f)
            return;

        // Put the tip slightly in front of the player
        Vector3 spawnPos = playerMovement.transform.position + (Vector3)(dir * innerOffset);
        GameObject hitboxObj = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        var hb = hitboxObj.GetComponent<MeleeHitbox>();
        if (hb != null)
        {
            hb.lifeTime = hitboxDuration;

            if (playerStats != null)
                hb.damage = playerStats.strength;
        }
    }

    private IEnumerator Cooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
