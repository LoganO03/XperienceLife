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

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (animController == null)
            animController = GetComponentInChildren<PlayerAnimationController>();
    }

    public void CastSpell()
    {
        Debug.Log("[Spell] CastSpell called");
        TryCastFireball();
    }

    private void TryCastFireball()
    {
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

        // Play spell animation
        if (animController != null)
        {
            Debug.Log("[Spell] PlaySpell()");
            animController.PlaySpell();
        }

        playerStats.currentMana -= fireballManaCost;

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

        float cd = GetFireballCooldown();
        StartCoroutine(FireballCooldownRoutine(cd));
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
