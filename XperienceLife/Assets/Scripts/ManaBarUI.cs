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
        if (playerStats == null) return;

        float ratio = 0f;
        if (playerStats.magic > 0f)
        {
            ratio = playerStats.currentMana / playerStats.magic;
        }

        fillImage.fillAmount = ratio;

        float current = Mathf.RoundToInt(playerStats.currentMana);
        float max = playerStats.magic;
        manaText.text = $"{current} / {max}";
    }
}
