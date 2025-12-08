using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

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
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        originalScale = transform.localScale;
    }

    public void TakeDamage(int amount)
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

        // flash color + scale punch
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
