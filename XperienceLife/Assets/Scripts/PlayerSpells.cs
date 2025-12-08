using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpells : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject fireballPrefab;

    [Header("Fireball Settings")]
    [SerializeField] private float fireballCooldownBase = 1.5f; // base cooldown in seconds
    [SerializeField] private int fireballManaCost = 10;
    [SerializeField] private float fireballSpawnOffset = 0.6f;

    private InputSystem_Actions controls;
    private bool canCastFire = true;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerStats>();

        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Spell.performed += OnCastFire;
    }

    private void OnDisable()
    {
        controls.Player.Spell.performed -= OnCastFire;
        controls.Disable();
    }

    private void OnCastFire(InputAction.CallbackContext ctx)
    {
        TryCastFireball();
    }

    private void TryCastFireball()
    {
        if (!canCastFire || fireballPrefab == null || playerMovement == null || playerStats == null)
            return;

        // Need enough mana
        if (playerStats.currentMana < fireballManaCost)
        {
            Debug.Log("Not enough mana for Fireball!");
            return;
        }

        Vector2 dir = playerMovement.AimDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        // Spend mana
        playerStats.currentMana -= fireballManaCost;

        // Spawn fireball in front of player
        Vector3 spawnPos = transform.position + (Vector3)(dir * fireballSpawnOffset);
        GameObject fbObj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        // Initialize projectile
        var projectile = fbObj.GetComponent<FireballProjectile>();
        if (projectile != null)
        {
            int bonusDamage = 0;

            // You can use Strength, Magic, or both as a damage buff
            if (playerStats != null)
                bonusDamage += playerStats.magic / 5; // example scaling

            projectile.Initialize(dir, bonusDamage);
        }

        // Start cooldown influenced by intellect
        float cd = GetFireballCooldown();
        StartCoroutine(FireballCooldownRoutine(cd));
    }

    private float GetFireballCooldown()
    {
        if (playerStats == null)
            return fireballCooldownBase;

        // Example: each point of Intellect reduces cooldown by 3%, capped
        float reduction = 0.03f * playerStats.intellect;
        reduction = Mathf.Clamp(reduction, 0f, 0.7f); // max 70% reduction
        float cd = fireballCooldownBase * (1f - reduction);
        return cd;
    }

    private System.Collections.IEnumerator FireballCooldownRoutine(float duration)
    {
        canCastFire = false;
        yield return new WaitForSeconds(duration);
        canCastFire = true;
    }
}
