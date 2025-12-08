using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;

    private void Awake()
    {
        // Make sure panel starts hidden
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(bool survived, int enemiesDefeated, float timeSeconds)
    {
        if (panel == null) return;

        panel.SetActive(true);

        titleText.text = survived ? "YOU SURVIVED" : "YOU DIED";

        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);

        statsText.text = $"Enemies defeated: {enemiesDefeated}\nTime survived: {minutes:00}:{seconds:00}";

        // Pause game time
        Time.timeScale = 0f;
    }

    // Hook this to the Quit button's OnClick
    public void OnQuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
