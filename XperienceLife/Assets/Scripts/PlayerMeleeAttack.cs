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

    // NEW: block window used when casting spells
    private bool blockFromSpell = false;

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
    /// Called by PlayerSpells to temporarily block melee when a spell is cast.
    /// </summary>
    public void BlockAttacksFor(float duration)
    {
        StartCoroutine(BlockAttacksRoutine(duration));
    }

    private IEnumerator BlockAttacksRoutine(float duration)
    {
        blockFromSpell = true;
        yield return new WaitForSeconds(duration);
        blockFromSpell = false;
    }

    public void PerformAttack()
    {
        Debug.Log("[Melee] PerformAttack called");

        // Block attacking while casting spells (animation-based)
        if (animController != null && animController.IsCasting)
        {
            Debug.Log("[Melee] Blocked attack because player is casting (anim)");
            return;
        }

        // EXTRA safety: block if we just cast a spell this frame
        if (blockFromSpell)
        {
            Debug.Log("[Melee] Blocked attack because of spell block window");
            return;
        }

        if (!canAttack || meleeHitboxPrefab == null || playerMovement == null)
            return;

        Vector2 dir = playerMovement.AimDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        if (animController != null)
        {
            Debug.Log("[Melee] PlayAttack()");
            animController.PlayAttack();
        }

        Vector3 spawnPos = playerMovement.transform.position + (Vector3)(dir * innerOffset);

        GameObject hitboxObj = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        hitboxObj.transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

        var hb = hitboxObj.GetComponent<MeleeHitbox>();
        if (hb != null)
        {
            hb.lifeTime = hitboxDuration;
            hb.damage = playerStats != null ? playerStats.GetMeleeDamage() : 1f;
        }

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
