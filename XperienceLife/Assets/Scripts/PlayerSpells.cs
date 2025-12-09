using UnityEngine;

public class PlayerSpells : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private PlayerAnimationController animController;
    [SerializeField] private PlayerMeleeAttack meleeAttack;  // NEW

    [Header("Fireball Settings")]
    [SerializeField] private float fireballManaCost = 10f;
    [SerializeField] private float fireballSpawnOffset = 0.6f;

    [Header("Cast Block Settings")]
    [SerializeField] private float meleeBlockWindow = 0.15f; // how long to block melee after casting

    private bool canCastFire = true;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        if (animController == null)
            animController = GetComponentInChildren<PlayerAnimationController>();

        if (meleeAttack == null)
            meleeAttack = GetComponent<PlayerMeleeAttack>();
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

        // BLOCK MELEE for a brief window so Spell input can't spawn a hitbox
        if (meleeAttack != null && meleeBlockWindow > 0f)
        {
            meleeAttack.BlockAttacksFor(meleeBlockWindow);
        }

        if (animController != null)
        {
            Debug.Log("[Spell] PlaySpell()");
            animController.PlaySpell();
        }

        playerStats.currentMana -= fireballManaCost;
        playerStats.currentMana = Mathf.Clamp(playerStats.currentMana, 0f, playerStats.maxMana);

        Vector3 spawnPos = transform.position + (Vector3)(dir * fireballSpawnOffset);
        GameObject fbObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        var projectile = fbObj.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            float damage = Mathf.Max(1f, playerStats.magic / 5f);
            projectile.Initialize(dir, damage);
        }

        float cd = GetFireballCooldown();
        StartCoroutine(FireballCooldownRoutine(cd));
    }

    private float GetFireballCooldown()
    {
        if (playerStats == null)
            return 5f;

        return playerStats.GetSpellCooldown();
    }

    private System.Collections.IEnumerator FireballCooldownRoutine(float duration)
    {
        canCastFire = false;
        yield return new WaitForSeconds(duration);
        canCastFire = true;
    }
}
