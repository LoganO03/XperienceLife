using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 3f;   // per-enemy base HP in inspector
    private float currentHealth;

    // store original so we can add difficulty on top
    private float baseMaxHealth;

    [Header("Hit Feedback")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float scalePunchAmount = 0.15f;

    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalScale;
    private bool isFlashing = false;

    private void Awake()
    {
        // remember original HP for difficulty scaling
        baseMaxHealth = maxHealth;

        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        originalScale = transform.localScale;
    }

    /// <summary>
    /// Called by spawner after Instantiate to apply wave-based difficulty.
    /// e.g., bonus = wave-1 so wave 1: +0, wave 2: +1, etc.
    /// </summary>
    public void ApplyDifficultyBonus(int bonus)
    {
        // keep baseMaxHealth as the per-enemy inspector value
        maxHealth = baseMaxHealth + bonus;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (!isFlashing)
            StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        isFlashing = true;

        if (sr != null)
            sr.color = hitColor;

        transform.localScale = originalScale * (1f + scalePunchAmount);

        yield return new WaitForSeconds(flashDuration);

        if (sr != null)
            sr.color = originalColor;

        transform.localScale = originalScale;
        isFlashing = false;
    }

    private void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterEnemyDefeated();

        Destroy(gameObject);
    }
}
