using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTimeSeconds = 15f * 60f; // 15 minutes

    public float TimeRemaining => timeRemaining;
    public float StartTimeSeconds => startTimeSeconds;
    public float ElapsedTime => startTimeSeconds - timeRemaining;

    private float timeRemaining;
    private bool isRunning = true;
    private bool notifiedEnd = false;

    private void Start()
    {
        timeRemaining = startTimeSeconds;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.unscaledDeltaTime; // so hitstop doesn't distort real time
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;

            if (!notifiedEnd && GameManager.Instance != null)
            {
                notifiedEnd = true;
                GameManager.Instance.OnTimerEnded();
            }
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
