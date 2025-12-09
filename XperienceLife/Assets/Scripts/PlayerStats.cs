using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Level Stats (these are your stat points / levels)")]
    [Tooltip("Health stat level. Effective HP = baseHealth + health.")]
    public int health = 0;

    [Tooltip("Stamina stat level. Effective stamina = floor(100 + 0.25 * stamina).")]
    public int stamina = 0;

    [Tooltip("Magic stat level. Controls max mana (and optionally spell damage).")]
    public int magic = 0;

    [Tooltip("Strength stat level. Effective melee damage = baseDamage + strength.")]
    public int strength = 0;

    [Tooltip("Intellect stat level. Reduces spell cooldown: 5s - 0.1s * intellect.")]
    public int intellect = 0;

    [Header("Runtime Resources (read-only at runtime)")]
    public float maxHealth;
    public float currentHealth;

    public float maxStamina;
    public float currentStamina;

    public float maxMana;
    public float currentMana;

    [Header("Config")]
    [SerializeField] private float baseHealth = 10f;               // base HP
    [SerializeField] private float baseStamina = 100f;             // base stamina
    [SerializeField] private float staminaPerLevel = 0.25f;        // +0.25 per stamina level (floored)

    [SerializeField] private float baseMana = 50f;                 // base mana
    [SerializeField] private float manaPerMagicLevel = 5f;         // +5 per magic level

    [SerializeField] private float baseDamage = 1f;                // base damage before strength
    [SerializeField] private float spellBaseCooldown = 5f;         // 5 seconds base
    [SerializeField] private float spellCooldownPerIntellect = 0.1f; // -0.1s per intellect level
    [SerializeField] private float minSpellCooldown = 0.25f;       // safety clamp

    private void Awake()
    {
        // On start, build all the derived stats and fill current values.
        RecalculateDerivedStats(resetCurrent: true);
    }

    private void OnValidate()
    {
        // Keep values consistent in the editor too.
        RecalculateDerivedStats(resetCurrent: false);
    }

    /// <summary>
    /// Recompute maxHealth, maxStamina, maxMana from levels. Optionally refill current.
    /// </summary>
    public void RecalculateDerivedStats(bool resetCurrent)
    {
        // Health: base 10 + 1 per health level
        maxHealth = baseHealth + health;

        if (resetCurrent || currentHealth > maxHealth)
            currentHealth = maxHealth;

        // Stamina: base 100 + 0.25 per stamina level, floored
        float rawStamina = baseStamina + stamina * staminaPerLevel;
        maxStamina = Mathf.Floor(rawStamina);

        if (resetCurrent || currentStamina > maxStamina)
            currentStamina = maxStamina;

        // Mana: base 50 + 5 per magic level
        maxMana = baseMana + magic * manaPerMagicLevel;

        if (resetCurrent || currentMana > maxMana)
            currentMana = maxMana;
    }

    /// <summary>
    /// Melee damage: base 1 + 1 per strength level.
    /// </summary>
    public float GetMeleeDamage()
    {
        return baseDamage + strength;
    }

    /// <summary>
    /// Spell cooldown: 5 seconds minus 0.1 seconds per intellect level (clamped).
    /// </summary>
    public float GetSpellCooldown()
    {
        float cd = spellBaseCooldown - spellCooldownPerIntellect * intellect;
        if (cd < minSpellCooldown)
            cd = minSpellCooldown;
        return cd;
    }
}
