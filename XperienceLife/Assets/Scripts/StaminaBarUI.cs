using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI staminaText;

    private void Update()
    {
        if (playerStats == null || fillImage == null || staminaText == null)
            return;

        float max = playerStats.maxStamina;

        float ratio = 0f;
        if (max > 0f)
        {
            ratio = playerStats.currentStamina / max;
        }

        fillImage.fillAmount = Mathf.Clamp01(ratio);

        int current = Mathf.RoundToInt(playerStats.currentStamina);
        int maxDisplay = Mathf.RoundToInt(max);

        staminaText.text = $"{current} / {maxDisplay}";
    }
}
