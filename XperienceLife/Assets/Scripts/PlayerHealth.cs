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

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    public int MaxHealth => stats != null ? stats.health : 5;
    public int CurrentHealth => stats != null ? stats.currentHealth : 0;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isFlashing = false;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (stats != null)
            stats.currentHealth = stats.health;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(int amount)
    {
        if (stats == null) return;

        stats.currentHealth -= amount;
        stats.currentHealth = Mathf.Clamp(stats.currentHealth, 0, stats.health);

        Debug.Log("Player took damage: " + amount + " (HP = " + stats.currentHealth + ")");

        if (!isFlashing)
            StartCoroutine(DamageFlash());

        StartCoroutine(HitStopRoutine());

        if (stats.currentHealth <= 0)
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
