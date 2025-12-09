using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI manaText;

    private void Update()
    {
        if (playerStats == null || fillImage == null || manaText == null)
            return;

        float max = playerStats.maxMana;

        float ratio = 0f;
        if (max > 0f)
        {
            ratio = playerStats.currentMana / max;
        }

        fillImage.fillAmount = Mathf.Clamp01(ratio);

        float current = Mathf.RoundToInt(playerStats.currentMana);
        manaText.text = $"{current} / {Mathf.RoundToInt(max)}";
    }
}
