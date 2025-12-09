using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public float health = 5f;            
    public float stamina = 100f;         
    public float magic = 50f;            
    public float strength = 1f;          
    public float intellect = 0f;         
    public float regen = 0f;             // HP per second
    public float manaRegen = 0f;         // mana per second

    [Header("Regen Delays")]
    public float healthRegenDelay = 3f;
    public float manaRegenDelay = 1f;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float currentMana;

    private Coroutine healthRegenCoroutine;
    private Coroutine manaRegenCoroutine;

    private void Awake()
    {
        currentHealth = health;
        currentStamina = stamina;
        currentMana = magic;
    }

    // -------- PUBLIC API -------- //

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, health);

        // restart regen delay
        if (healthRegenCoroutine != null)
            StopCoroutine(healthRegenCoroutine);

        healthRegenCoroutine = StartCoroutine(HealthRegenRoutine());
    }

    public bool TrySpendMana(float amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0f, magic);

        // restart mana regen delay
        if (manaRegenCoroutine != null)
            StopCoroutine(manaRegenCoroutine);

        manaRegenCoroutine = StartCoroutine(ManaRegenRoutine());

        return true;
    }

    // ---------- COROUTINES ---------- //

    private IEnumerator HealthRegenRoutine()
    {
        // wait for delay
        yield return new WaitForSeconds(healthRegenDelay);

        // regen loop
        while (currentHealth < health)
        {
            currentHealth += regen * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, health);
            yield return null;
        }

        healthRegenCoroutine = null;
    }

    private IEnumerator ManaRegenRoutine()
    {
        // wait for delay
        yield return new WaitForSeconds(manaRegenDelay);

        // regen loop
        while (currentMana < magic)
        {
            currentMana += manaRegen * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0f, magic);
            yield return null;
        }

        manaRegenCoroutine = null;
    }
}
