using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerStats stats;

    [Header("Damage Flash")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Hitstop")]
    [SerializeField] private float hitStopDuration = 0.05f;
    [SerializeField] private float hitStopTimeScale = 0.2f;

    public float MaxHealth => stats != null ? stats.maxHealth : 10f;
    public float CurrentHealth => stats != null ? stats.currentHealth : 0f;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isFlashing = false;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (stats != null)
        {
            // Ensure derived stats are up to date and current HP is filled
            stats.RecalculateDerivedStats(true);
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        if (stats == null) return;

        stats.currentHealth -= amount;
        stats.currentHealth = Mathf.Clamp(stats.currentHealth, 0f, stats.maxHealth);

        Debug.Log("Player took damage: " + amount + " (HP = " +
                  stats.currentHealth + " / " + stats.maxHealth + ")");

        if (!isFlashing)
            StartCoroutine(DamageFlash());

        StartCoroutine(HitStopRoutine());

        if (stats.currentHealth <= 0f)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        isFlashing = true;

        if (sr != null)
            sr.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        if (sr != null)
            sr.color = originalColor;

        isFlashing = false;
    }

    private System.Collections.IEnumerator HitStopRoutine()
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = hitStopTimeScale;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = originalTimeScale;
    }

    private void Die()
    {
        Debug.Log("Player died!");

        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerDied();
    }
}
