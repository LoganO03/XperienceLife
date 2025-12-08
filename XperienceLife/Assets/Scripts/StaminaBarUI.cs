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
        if (playerStats == null) return;

        float ratio = 0f;
        if (playerStats.stamina > 0f)
        {
            ratio = playerStats.currentStamina / playerStats.stamina;
        }

        fillImage.fillAmount = ratio;

        int current = Mathf.RoundToInt(playerStats.currentStamina);
        int max = Mathf.RoundToInt(playerStats.stamina);
        staminaText.text = $"{current} / {max}";
    }
}
