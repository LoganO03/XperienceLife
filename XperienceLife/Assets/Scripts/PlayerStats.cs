using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int health = 5;           // max HP
    public int stamina = 100;        // max stamina (for sprint)
    public int magic = 50;           // max mana
    public int strength = 1;         // melee damage multiplier or base
    public int intellect = 0;        // reduces spell cooldowns
    public float regen = 0f;         // HP per second
    public float manaRegen = 0f;     // mana per second

    // We’ll track current values here
    [HideInInspector] public int currentHealth;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float currentMana;

    private void Awake()
    {
        currentHealth = health;
        currentStamina = stamina;
        currentMana = magic;
    }

    private void Update()
    {
        // Regen HP
        if (regen > 0f && currentHealth < health)
        {
            float newHP = currentHealth + regen * Time.deltaTime;
            currentHealth = Mathf.Clamp(Mathf.FloorToInt(newHP), 0, health);
        }

        // Regen Mana
        if (manaRegen > 0f && currentMana < magic)
        {
            currentMana += manaRegen * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, magic);
        }

        // Stamina regen (we’ll wire up sprint later)
        // For now, just cap it so it doesn’t go out of range
        currentStamina = Mathf.Clamp(currentStamina, 0, stamina);
    }
}
