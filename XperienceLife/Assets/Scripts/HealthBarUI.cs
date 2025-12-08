using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Update()
    {
        if (playerHealth == null) return;

        float ratio = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        fillImage.fillAmount = ratio;

        healthText.text = $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";
    }
}
