using UnityEngine;

public class PlayerSpells : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private PlayerAnimationController animController;

    [Header("Fireball Settings")]
    [SerializeField] private float fireballCooldownBase = 1.5f;
    [SerializeField] private float fireballManaCost = 10f;
    [SerializeField] private float fireballSpawnOffset = 0.6f;

    private bool canCastFire = true;

    // queued data for the animation event
    private Vector2 queuedDir = Vector2.right;
    private bool hasQueuedFireball = false;

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
    /// Called from input. Starts the cast animation and queues the shot,
    /// but does NOT spawn the fireball yet.
    /// </summary>
    public void CastSpell()
    {
        Debug.Log("[Spell] CastSpell (request) called");

        if (!canCastFire || fireballPrefab == null || playerMovement == null || playerStats == null)
            return;

        if (playerStats.currentMana < fireballManaCost)
        {
            Debug.Log("[Spell] Not enough mana");
            return;
        }

        Vector2 dir = playerMovement.AimDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        queuedDir = dir.normalized;
        hasQueuedFireball = true;

        // play spell animation
        if (animController != null)
        {
            Debug.Log("[Spell] PlaySpell()");
            animController.PlaySpell();
        }

        // start cooldown at cast start
        float cd = GetFireballCooldown();
        StartCoroutine(FireballCooldownRoutine(cd));
    }

    /// <summary>
    /// Animation Event – call this from the PSpell clip
    /// at the frame where the projectile should be emitted.
    /// </summary>
    public void Animation_FireballCast()
    {
        Debug.Log("[Spell] Animation_FireballCast event fired");

        if (!hasQueuedFireball)
            return;

        hasQueuedFireball = false;

        if (playerStats == null || fireballPrefab == null || playerMovement == null)
            return;

        if (playerStats.currentMana < fireballManaCost)
        {
            Debug.Log("[Spell] Mana spent or changed before event – cancel cast");
            return;
        }

        // spend mana at the moment of cast
        playerStats.currentMana -= fireballManaCost;

        Vector2 dir = queuedDir;
        if (dir.sqrMagnitude < 0.001f)
            dir = playerMovement.AimDirection.normalized;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Vector3 spawnPos = transform.position + (Vector3)(dir * fireballSpawnOffset);
        GameObject fbObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        var projectile = fbObj.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            float bonusDamage = 0f;
            if (playerStats != null)
                bonusDamage += playerStats.magic / 5f;

            projectile.Initialize(dir, bonusDamage);
        }
    }

    private float GetFireballCooldown()
    {
        if (playerStats == null)
            return fireballCooldownBase;

        float reduction = 0.03f * playerStats.intellect;
        reduction = Mathf.Clamp(reduction, 0f, 0.7f);
        return fireballCooldownBase * (1f - reduction);
    }

    private System.Collections.IEnumerator FireballCooldownRoutine(float duration)
    {
        canCastFire = false;
        yield return new WaitForSeconds(duration);
        canCastFire = true;
    }
}
